using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Signatures;

namespace Cartorio.Documentos
{
    public class AssinaturaVerificada
    {
        public string NomeCampo { get; set; }
        public string Assinante { get; set; }
        public DateTime DataAssinatura { get; set; }

        /// <summary>O conteúdo assinado não foi alterado e a assinatura confere com o certificado.</summary>
        public bool Integra { get; set; }

        /// <summary>Falso quando alguém acrescentou conteúdo ao PDF depois da assinatura.</summary>
        public bool CobreDocumentoInteiro { get; set; }

        public bool Valida => Integra && CobreDocumentoInteiro;
    }

    /// <summary>
    /// Confere integridade das assinaturas de um PDF. Não valida a cadeia de confiança
    /// (isso exigiria a raiz ICP-Brasil); só garante que o documento não mudou.
    /// </summary>
    public static class VerificadorAssinaturaPdf
    {
        public static IList<AssinaturaVerificada> Verificar(byte[] pdf)
        {
            var resultado = new List<AssinaturaVerificada>();

            using (var stream = new MemoryStream(pdf))
            using (var documento = new PdfDocument(new PdfReader(stream)))
            {
                var util = new SignatureUtil(documento);
                foreach (var nome in util.GetSignatureNames())
                {
                    var pkcs7 = util.ReadSignatureData(nome);
                    resultado.Add(new AssinaturaVerificada
                    {
                        NomeCampo = nome,
                        Assinante = pkcs7.GetSigningCertificate().GetSubjectDN().ToString(),
                        DataAssinatura = pkcs7.GetSignDate(),
                        Integra = pkcs7.VerifySignatureIntegrityAndAuthenticity(),
                        CobreDocumentoInteiro = util.SignatureCoversWholeDocument(nome)
                    });
                }
            }

            return resultado;
        }
    }
}
