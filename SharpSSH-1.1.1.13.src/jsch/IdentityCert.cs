using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Tamir.SharpSsh.jsch
{
    /// <summary>
    /// Support X.509 certificate storage of private key streams.  Password protected streams
    /// are not supported.  Certs should be stored in personal store of the current security context.
    /// </summary>
    internal class IdentityCert : Identity
    {
        private const string ALGORITHM_NAME = "ssh-rsa";

        private string _identity;
        private JSch _jsch;

        private byte[] _n_array;   // modulus
        private byte[] _e_array;   // public exponent
        private byte[] _d_array;   // private exponent

        private byte[] _p_array;
        private byte[] _q_array;
        private byte[] _dmp1_array;
        private byte[] _dmq1_array;
        private byte[] _iqmp_array;

        private byte[] _publickeyBlob = null;

        internal IdentityCert(String identity, JSch jsch)
        {
            this._identity = identity;
            this._jsch = jsch;
            X509Store certStore = null;
            bool foundPrivateKey = false;
            string thumbprint = identity.ToUpperInvariant().Replace(" ", "");
            try
            {
                certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true))
                {
                    RSACryptoServiceProvider privateKey = certificate.PrivateKey as RSACryptoServiceProvider;
                    if (privateKey != null)
                    {
                        RSAParameters rsaKeyInfo = privateKey.ExportParameters(true);
                        List<byte> modulus = new List<byte>(1 + rsaKeyInfo.Modulus.Length);
                        modulus.Add(0);
                        modulus.AddRange(rsaKeyInfo.Modulus);
                        _n_array = modulus.ToArray();
                        _d_array = rsaKeyInfo.D;
                        _dmp1_array = rsaKeyInfo.DP;
                        _dmq1_array = rsaKeyInfo.DQ;
                        _e_array = rsaKeyInfo.Exponent;
                        _iqmp_array = rsaKeyInfo.InverseQ;
                        _p_array = rsaKeyInfo.P;
                        _q_array = rsaKeyInfo.Q;
                        foundPrivateKey = true;
                    }
                    break;
                }

                if (!foundPrivateKey) throw new JSchException("privatekey not found: " + identity);
            }
            catch (Exception e)
            {
                Console.WriteLine("Identity: " + e);
                if (e is JSchException) throw (JSchException)e;
                throw new JSchException(e.ToString());
            }
            finally
            {
                if (certStore != null)
                {
                    certStore.Close();
                }
            }
        }

        public String getAlgName()
        {
            return ALGORITHM_NAME;
        }

        public bool setPassphrase(String _passphrase)
        {
            return true;
        }

        public byte[] getPublicKeyBlob()
        {
            if (_e_array == null)
            {
                return null;
            }
            else if (_publickeyBlob == null)
            {
                Buffer buf = new Buffer(ALGORITHM_NAME.Length + 4 + _e_array.Length + 4 + _n_array.Length + 4);
                buf.putString(System.Text.Encoding.Default.GetBytes(ALGORITHM_NAME));
                buf.putString(_e_array);
                buf.putString(_n_array);
                _publickeyBlob = buf.buffer;
            }
            return _publickeyBlob;
        }

        public byte[] getSignature(Session session, byte[] data)
        {
            try
            {
                SignatureRSA rsa = new Tamir.SharpSsh.jsch.jce.SignatureRSA();
                rsa.init();
                rsa.setPrvKey(_e_array, _n_array, _d_array, _p_array, _q_array, _dmp1_array, _dmq1_array, _iqmp_array);
                rsa.update(data);
                byte[] sig = rsa.sign();
                Buffer buf = new Buffer(ALGORITHM_NAME.Length + 4 + sig.Length + 4);
                buf.putString(System.Text.Encoding.Default.GetBytes(ALGORITHM_NAME));
                buf.putString(sig);
                return buf.buffer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public bool decrypt()
        {
            return true;
        }

        public bool isEncrypted()
        {
            return false;
        }

        public String getName()
        {
            return _identity;
        }
    }
}