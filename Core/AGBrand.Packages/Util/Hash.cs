using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace AGBrand.Packages.Util
{
    public class Hash : IDisposable
    {
        private HashAlgorithm _mCryptoService;
        private bool isDisposed;

        public Hash()
        {
            _mCryptoService = new SHA1Managed();
        }

        public Hash(HashServiceProvider serviceProvider)
        {
            _mCryptoService = serviceProvider switch
            {
                HashServiceProvider.Md5 => new MD5CryptoServiceProvider(),
                HashServiceProvider.Sha1 => new SHA1Managed(),
                HashServiceProvider.Sha256 => new SHA256Managed(),
                HashServiceProvider.Sha384 => new SHA384Managed(),
                HashServiceProvider.Sha512 => new SHA512Managed(),
                _ => throw new ArgumentOutOfRangeException(nameof(serviceProvider), serviceProvider, null),
            };
        }

        public Hash(string serviceProviderName)
        {
            _mCryptoService = (HashAlgorithm)CryptoConfig.CreateFromName(serviceProviderName.ToUpper(CultureInfo.InvariantCulture));
        }

        public enum HashServiceProvider
        {
            Sha1,
            Sha256,
            Sha384,
            Sha512,
            Md5
        }

        public string Salt
        {
            get;
            set;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual string Encrypt(string plainText)
        {
            var cryptoByte = _mCryptoService.ComputeHash(Encoding.ASCII.GetBytes(plainText + Salt));

            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.Length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (_mCryptoService == null)
            {
                return;
            }

            if (disposing)
            {
                _mCryptoService.Dispose();
                _mCryptoService = null;
            }

            isDisposed = true;
        }
    }
}
