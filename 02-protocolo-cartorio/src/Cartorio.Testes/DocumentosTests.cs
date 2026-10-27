using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cartorio.Documentos;
using Cartorio.Entidades;
using Cartorio.Negocio;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cartorio.Testes
{
    /// <summary>Integração: RDLC, PDF e assinatura reais, com certificado autoassinado criado em memória.</summary>
    [TestClass]
    public class DocumentosTests
    {
        private const string Senha = "senha-de-teste";
        private const string Titular = "Cartorio de Notas de Teste";
        private static readonly DateTime Agora = new DateTime(2026, 10, 27, 15, 30, 0);

        private static EmissorComprovante _emissor;
        private static GeradorRelatorioProtocolos _gerador;

        [ClassInitialize]
        public static void InicializarClasse(TestContext contexto)
        {
            var certificado = CertificadoAssinatura.CarregarPfx(CertificadoAssinatura.CriarPfxAutoassinado(Titular, Senha), Senha);
            _emissor = new EmissorComprovante(new AssinadorPdf(certificado), () => Agora);
            _gerador = new GeradorRelatorioProtocolos(() => Agora);
        }

        private static Protocolo Protocolo(StatusProtocolo status = StatusProtocolo.Recebido, string numero = "2026-000042")
        {
            return new Protocolo
            {
                Id = 42,
                Numero = numero,
                Cliente = new Cliente { Id = 1, Nome = "Maria Aparecida Souza", Cpf = "52998224725" },
                TipoServico = "Procuração",
                Descricao = "Venda de veículo",
                Status = status,
                DataAbertura = new DateTime(2026, 10, 20, 9, 15, 0),
                DataAtualizacao = new DateTime(2026, 10, 25, 16, 0, 0),
                DataEntrega = status == StatusProtocolo.Entregue ? new DateTime(2026, 10, 25, 16, 0, 0) : (DateTime?)null
            };
        }

        #region Comprovante

        [TestMethod]
        public void ComprovanteDeAbertura_TrazOsDadosDoProtocolo()
        {
            var texto = ExtrairTexto(_emissor.RenderizarPdf(Protocolo(), TipoComprovante.Abertura));

            StringAssert.Contains(texto, "COMPROVANTE DE PROTOCOLO");
            StringAssert.Contains(texto, "2026-000042");
            StringAssert.Contains(texto, "Maria Aparecida Souza");
            StringAssert.Contains(texto, "529.982.247-25");
            StringAssert.Contains(texto, "20/10/2026 09:15");
            Assert.IsFalse(texto.Contains("Recebido por"), "a linha de recebimento é só do comprovante de entrega");
        }

        [TestMethod]
        public void ComprovanteDeEntrega_TrazDataDeEntregaELinhaDeRecebimento()
        {
            var texto = ExtrairTexto(_emissor.RenderizarPdf(Protocolo(StatusProtocolo.Entregue), TipoComprovante.Entrega));

            StringAssert.Contains(texto, "COMPROVANTE DE ENTREGA");
            StringAssert.Contains(texto, "25/10/2026 às 16:00");
            StringAssert.Contains(texto, "Recebido por");
        }

        [TestMethod]
        public void Comprovante_SaiAssinadoPeloTitularEIntegro()
        {
            var pdf = _emissor.GerarPdfAssinado(Protocolo(), TipoComprovante.Abertura);

            var assinatura = VerificadorAssinaturaPdf.Verificar(pdf).Single();

            Assert.IsTrue(assinatura.Valida);
            StringAssert.Contains(assinatura.Assinante, "CN=" + Titular);
        }

        [TestMethod]
        public void Comprovante_AlteradoDepoisDeAssinado_EhDetectado()
        {
            var assinado = _emissor.GerarPdfAssinado(Protocolo(), TipoComprovante.Abertura);

            byte[] adulterado;
            using (var entrada = new MemoryStream(assinado))
            using (var saida = new MemoryStream())
            {
                using (var documento = new PdfDocument(new PdfReader(entrada), new PdfWriter(saida), new StampingProperties().UseAppendMode()))
                using (var layout = new Document(documento))
                {
                    layout.Add(new Paragraph("Cliente: Outra Pessoa"));
                }
                adulterado = saida.ToArray();
            }

            Assert.IsFalse(VerificadorAssinaturaPdf.Verificar(adulterado).Single().Valida);
        }

        [TestMethod]
        public void Comprovante_SemClienteCarregado_LancaArgumentException()
        {
            var protocolo = Protocolo();
            protocolo.Cliente = null;

            Assert.ThrowsException<ArgumentException>(() => _emissor.RenderizarPdf(protocolo, TipoComprovante.Abertura));
        }

        [TestMethod]
        public void NomeSugerido_IndicaOTipoEONumero()
        {
            Assert.AreEqual("comprovante-protocolo-2026-000042.pdf", EmissorComprovante.NomeSugerido(Protocolo(), TipoComprovante.Abertura));
            Assert.AreEqual("comprovante-entrega-2026-000042.pdf", EmissorComprovante.NomeSugerido(Protocolo(), TipoComprovante.Entrega));
        }

        #endregion

        #region Relatório

        [TestMethod]
        public void Relatorio_ListaOsProtocolosEOTotal()
        {
            var protocolos = new List<Protocolo>
            {
                Protocolo(StatusProtocolo.Recebido, "2026-000001"),
                Protocolo(StatusProtocolo.Pronto, "2026-000002"),
                Protocolo(StatusProtocolo.Entregue, "2026-000003")
            };

            var texto = ExtrairTexto(_gerador.RenderizarPdf(protocolos, "Filtro de teste"));

            StringAssert.Contains(texto, "RELATÓRIO DE PROTOCOLOS");
            StringAssert.Contains(texto, "Filtro de teste");
            StringAssert.Contains(texto, "2026-000001");
            StringAssert.Contains(texto, "2026-000003");
            StringAssert.Contains(texto, "Total de protocolos: 3");
            StringAssert.Contains(texto, "Página 1 de 1");
        }

        [TestMethod]
        public void Relatorio_SemProtocolos_MostraMensagem()
        {
            var texto = ExtrairTexto(_gerador.RenderizarPdf(new List<Protocolo>(), "Filtro"));

            StringAssert.Contains(texto, "Nenhum protocolo encontrado");
            StringAssert.Contains(texto, "Total de protocolos: 0");
        }

        [TestMethod]
        public void Relatorio_ComMuitosProtocolos_QuebraEmPaginas()
        {
            var protocolos = Enumerable.Range(1, 120)
                .Select(i => Protocolo(StatusProtocolo.Recebido, "2026-" + i.ToString("000000")))
                .ToList();

            var pdf = _gerador.RenderizarPdf(protocolos, "Filtro");

            using (var stream = new MemoryStream(pdf))
            using (var documento = new PdfDocument(new PdfReader(stream)))
                Assert.IsTrue(documento.GetNumberOfPages() > 1);
        }

        [TestMethod]
        public void DescreverFiltro_MostraPeriodoEStatus()
        {
            Assert.AreEqual("Abertos de 01/10/2026 a 27/10/2026 | Status: Em análise",
                GeradorRelatorioProtocolos.DescreverFiltro(new DateTime(2026, 10, 1), new DateTime(2026, 10, 27), StatusProtocolo.EmAnalise));
            StringAssert.EndsWith(
                GeradorRelatorioProtocolos.DescreverFiltro(new DateTime(2026, 10, 1), new DateTime(2026, 10, 27), null),
                "Status: todos");
        }

        #endregion

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
