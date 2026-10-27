using System;
using System.IO;
using System.Linq;
using System.Text;
using Emolumentos.Apresentacao;
using Emolumentos.Documentos;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emolumentos.Testes
{
    /// <summary>Integração: RDLC, PDF e assinatura reais, com certificado autoassinado criado em memória.</summary>
    [TestClass]
    public class EmissaoDocumentoTests
    {
        private const string Senha = "senha-de-teste";
        private const string Titular = "Cartorio de Teste";

        private static EmissorDemonstrativo _emissor;

        [ClassInitialize]
        public static void InicializarClasse(TestContext contexto)
        {
            var certificado = CertificadoAssinatura.CarregarPfx(CertificadoAssinatura.CriarPfxAutoassinado(Titular, Senha), Senha);
            _emissor = new EmissorDemonstrativo(new AssinadorPdf(certificado), () => new DateTime(2026, 10, 27, 15, 30, 0));
        }

        private static ResultadoCalculoViewModel Resultado()
        {
            return new ResultadoCalculoViewModel
            {
                Uf = "SP",
                TipoAto = "Escritura pública com valor declarado",
                ValorNegocio = "R$ 150.000,00",
                Faixa = "De R$ 100.000,01 até R$ 500.000,00",
                Emolumento = "R$ 4.100,00",
                Custas = "R$ 1.300,00",
                Total = "R$ 5.400,00"
            };
        }

        [TestMethod]
        public void Rdlc_RenderizaUmPdfComOsDadosDoCalculo()
        {
            var pdf = _emissor.RenderizarPdf(Resultado());

            Assert.AreEqual("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));

            var texto = ExtrairTexto(pdf);
            StringAssert.Contains(texto, "DEMONSTRATIVO DE EMOLUMENTOS");
            StringAssert.Contains(texto, "R$ 5.400,00");
            StringAssert.Contains(texto, "27/10/2026 15:30");
            StringAssert.Contains(texto, Titular);
        }

        [TestMethod]
        public void ConfigurarRelatorio_PreparaOLocalReportUsadoNaPrevia()
        {
            using (var relatorio = new Microsoft.Reporting.WinForms.LocalReport())
            {
                _emissor.ConfigurarRelatorio(relatorio, Resultado());

                var parametros = relatorio.GetParameters();
                Assert.AreEqual("R$ 5.400,00", parametros["Total"].Values[0]);
                Assert.AreEqual(Titular, parametros["Assinante"].Values[0]);
            }
        }

        [TestMethod]
        public void PdfEmitido_TemUmaAssinaturaValidaDoTitular()
        {
            var pdf = _emissor.GerarPdfAssinado(Resultado());

            var assinatura = VerificadorAssinaturaPdf.Verificar(pdf).Single();

            Assert.IsTrue(assinatura.Integra, "o conteúdo assinado deveria estar íntegro");
            Assert.IsTrue(assinatura.CobreDocumentoInteiro, "a assinatura deveria cobrir o documento inteiro");
            StringAssert.Contains(assinatura.Assinante, "CN=" + Titular);
        }

        [TestMethod]
        public void PdfSemAssinatura_NaoTemAssinaturas()
        {
            var pdf = _emissor.RenderizarPdf(Resultado());

            Assert.AreEqual(0, VerificadorAssinaturaPdf.Verificar(pdf).Count);
        }

        [TestMethod]
        public void ConteudoAcrescentadoDepoisDaAssinatura_EhDetectado()
        {
            var assinado = _emissor.GerarPdfAssinado(Resultado());

            // Simula uma adulteração: alguém reabre o PDF e acrescenta um texto (atualização incremental).
            byte[] adulterado;
            using (var entrada = new MemoryStream(assinado))
            using (var saida = new MemoryStream())
            {
                using (var documento = new PdfDocument(new PdfReader(entrada), new PdfWriter(saida), new StampingProperties().UseAppendMode()))
                using (var layout = new Document(documento))
                {
                    layout.Add(new Paragraph("TOTAL A RECOLHER: R$ 1,00"));
                }
                adulterado = saida.ToArray();
            }

            var assinatura = VerificadorAssinaturaPdf.Verificar(adulterado).Single();

            Assert.IsFalse(assinatura.CobreDocumentoInteiro);
            Assert.IsFalse(assinatura.Valida);
        }

        [TestMethod]
        public void Emitir_GravaOArquivoNoCaminhoInformado()
        {
            var caminho = Path.Combine(Path.GetTempPath(), "demonstrativo-" + Guid.NewGuid() + ".pdf");
            try
            {
                _emissor.Emitir(Resultado(), caminho);

                Assert.IsTrue(File.Exists(caminho));
                Assert.IsTrue(VerificadorAssinaturaPdf.Verificar(File.ReadAllBytes(caminho)).Single().Valida);
            }
            finally
            {
                File.Delete(caminho);
            }
        }

        [TestMethod]
        public void ObterOuCriarDeTeste_CriaOPfxUmaVezEDepoisReaproveita()
        {
            var caminho = Path.Combine(Path.GetTempPath(), "cert-" + Guid.NewGuid() + ".pfx");
            try
            {
                CertificadoAssinatura.ObterOuCriarDeTeste(caminho, Senha, Titular);
                var bytesCriados = File.ReadAllBytes(caminho);

                var segundo = CertificadoAssinatura.ObterOuCriarDeTeste(caminho, Senha, Titular);

                CollectionAssert.AreEqual(bytesCriados, File.ReadAllBytes(caminho));
                Assert.AreEqual(Titular, segundo.NomeTitular);
            }
            finally
            {
                File.Delete(caminho);
            }
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
