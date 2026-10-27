using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using Org.BouncyCastle.Pkcs;

namespace Legado.Documentos
{
    /// <summary>
    /// Chave privada e cadeia lidas de um PFX (PKCS#12). Em produção seria um certificado
    /// ICP-Brasil A1; aqui é um autoassinado de teste.
    /// </summary>
    public class CertificadoAssinatura
    {
        private CertificadoAssinatura(IPrivateKey chavePrivada, IX509Certificate[] cadeia, string nomeTitular)
        {
            ChavePrivada = chavePrivada;
            Cadeia = cadeia;
            NomeTitular = nomeTitular;
        }

        public IPrivateKey ChavePrivada { get; }
        public IX509Certificate[] Cadeia { get; }

        /// <summary>CN do certificado, ex.: "Cartório de Teste - Portfólio".</summary>
        public string NomeTitular { get; }

        public static CertificadoAssinatura CarregarPfx(byte[] pfx, string senha)
        {
            var store = new Pkcs12StoreBuilder().Build();
            using (var stream = new MemoryStream(pfx))
                store.Load(stream, (senha ?? string.Empty).ToCharArray());

            var alias = store.Aliases.Cast<string>().FirstOrDefault(store.IsKeyEntry);
            if (alias == null)
                throw new InvalidOperationException("O arquivo PFX não contém chave privada.");

            var chave = new PrivateKeyBC(store.GetKey(alias).Key);
            var cadeia = store.GetCertificateChain(alias)
                .Select(entrada => (IX509Certificate)new X509CertificateBC(entrada.Certificate))
                .ToArray();

            var titular = store.GetCertificateChain(alias)[0].Certificate.SubjectDN
                .GetValueList(Org.BouncyCastle.Asn1.X509.X509Name.CN)
                .Cast<string>()
                .FirstOrDefault() ?? "Desconhecido";

            return new CertificadoAssinatura(chave, cadeia, titular);
        }

        public static CertificadoAssinatura CarregarPfx(string caminho, string senha)
        {
            return CarregarPfx(File.ReadAllBytes(caminho), senha);
        }

        /// <summary>
        /// Carrega o PFX do caminho; se ele não existir, cria um certificado autoassinado de teste ali.
        /// Só para desenvolvimento e demonstração.
        /// </summary>
        public static CertificadoAssinatura ObterOuCriarDeTeste(string caminho, string senha, string nomeTitular)
        {
            if (!File.Exists(caminho))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(caminho)));
                File.WriteAllBytes(caminho, CriarPfxAutoassinado(nomeTitular, senha));
            }
            return CarregarPfx(caminho, senha);
        }

        /// <summary>Gera um PFX autoassinado (RSA 2048, SHA-256, validade de 2 anos).</summary>
        public static byte[] CriarPfxAutoassinado(string nomeTitular, string senha)
        {
            using (var rsa = RSA.Create(2048))
            {
                var requisicao = new CertificateRequest(
                    "CN=" + nomeTitular + ", O=Projeto de portfolio, C=BR",
                    rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                requisicao.CertificateExtensions.Add(new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, true));

                var agora = DateTimeOffset.Now;
                using (var certificado = requisicao.CreateSelfSigned(agora.AddDays(-1), agora.AddYears(2)))
                    return certificado.Export(X509ContentType.Pfx, senha);
            }
        }
    }
}
