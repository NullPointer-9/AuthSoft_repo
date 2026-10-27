using System;
using System.IO;
using System.Linq;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Legado.Apresentacao;
using Legado.Documentos;
using Legado.Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Legado.Testes
{
    /// <summary>Integração: RDLC, PDF e assinatura de verdade, com certificado autoassinado criado em memória.</summary>
    [TestClass]
    public class EmissaoReciboTests
    {
        private const string Titular = "Balcao de Teste";
        private static EmissorRecibo _emissor;

        [ClassInitialize]
        public static void InicializarClasse(TestContext contexto)
        {
            var certificado = CertificadoAssinatura.CarregarPfx(CertificadoAssinatura.CriarPfxAutoassinado(Titular, "x"), "x");
            _emissor = new EmissorRecibo(new AssinadorPdf(certificado), () => new DateTime(2026, 10, 27, 16, 0, 0));
        }

        private static ReciboViewModel Recibo()
        {
            var pedido = new PedidoCertidao
            {
                Id = 7,
                IdCliente = 1,
                Tipo = "Certidao de obito",
                Valor = 78.53m,
                Status = StatusPedido.Pago,
                DataPedido = new DateTime(2026, 10, 20)
            };
            return new ReciboViewModel(pedido, new Cliente { Id = 1, Nome = "Maria Aparecida Souza", Cpf = "52998224725" });
        }

        [TestMethod]
        public void Recibo_TrazClienteValorEPedido()
        {
            var texto = ExtrairTexto(_emissor.RenderizarPdf(Recibo()));

            StringAssert.Contains(texto, "RECIBO DE PEDIDO DE CERTIDÃO");
            StringAssert.Contains(texto, "Maria Aparecida Souza");
            StringAssert.Contains(texto, "529.982.247-25");
            StringAssert.Contains(texto, "78,53");
            StringAssert.Contains(texto, "Pedido nº 7");
            StringAssert.Contains(texto, "20/10/2026");
        }

        [TestMethod]
        public void Recibo_SaiAssinadoEIntegro()
        {
            var assinatura = VerificadorAssinaturaPdf.Verificar(_emissor.GerarPdfAssinado(Recibo())).Single();

            Assert.IsTrue(assinatura.Valida);
            StringAssert.Contains(assinatura.Assinante, "CN=" + Titular);
        }

        [TestMethod]
        public void NomeSugerido_UsaONumeroDoPedido()
        {
            Assert.AreEqual("recibo-pedido-7.pdf", Recibo().NomeArquivoSugerido);
        }

        private static string ExtrairTexto(byte[] pdf)
        {
            using (var stream = new MemoryStream(pdf))
            using (var documento = new PdfDocument(new PdfReader(stream)))
            {
                var texto = new StringBuilder();
                for (var pagina = 1; pagina <= documento.GetNumberOfPages(); pagina++)
                    texto.AppendLine(PdfTextExtractor.GetTextFromPage(documento.GetPage(pagina)));
                return texto.ToString();
            }
        }
    }
}
