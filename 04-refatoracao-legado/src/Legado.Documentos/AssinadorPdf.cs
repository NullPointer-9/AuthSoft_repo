using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Forms.Form.Element;
using iText.Signatures;

namespace Legado.Documentos
{
    /// <summary>
    /// Assina um PDF no padrão PAdES (assinatura CAdES destacada embutida no PDF),
    /// com um carimbo visível na primeira página.
    /// </summary>
    public class AssinadorPdf
    {
        private readonly CertificadoAssinatura _certificado;

        public AssinadorPdf(CertificadoAssinatura certificado)
        {
            _certificado = certificado ?? throw new ArgumentNullException(nameof(certificado));
        }

        public string NomeTitular => _certificado.NomeTitular;

        public byte[] Assinar(byte[] pdf, string motivo, string local)
        {
            using (var entrada = new MemoryStream(pdf))
            using (var saida = new MemoryStream())
            {
                var signer = new PdfSigner(new PdfReader(entrada), saida, new StampingProperties());

                var dataAssinatura = DateTime.Now;
                var carimbo = new SignatureFieldAppearance(SignerProperties.IGNORED_ID)
                    .SetContent(
                        "Assinado digitalmente por " + _certificado.NomeTitular + "\n" +
                        "Data: " + dataAssinatura.ToString("dd/MM/yyyy HH:mm:ss") + "\n" +
                        "Motivo: " + motivo);

                var propriedades = new SignerProperties()
                    .SetFieldName("AssinaturaCartorio")
                    .SetReason(motivo)
                    .SetLocation(local)
                    .SetClaimedSignDate(dataAssinatura)
                    .SetPageNumber(1)
                    // Canto inferior esquerdo da página A4 (coordenadas em pontos, origem embaixo).
                    .SetPageRect(new Rectangle(57, 57, 300, 60))
                    .SetSignatureAppearance(carimbo);
                signer.SetSignerProperties(propriedades);

                var assinatura = new PrivateKeySignature(_certificado.ChavePrivada, "SHA-256");
                signer.SignDetached(new BouncyCastleDigest(), assinatura, _certificado.Cadeia,
                    null, null, null, 0, PdfSigner.CryptoStandard.CADES);

                return saida.ToArray();
            }
        }
    }
}
