$certificate = New-SelfSignedCertificate `
    -Subject agbrand `
    -DnsName agbrand.com `
    -KeyAlgorithm RSA `
    -KeyLength 2048 `
    -NotBefore (Get-Date) `
    -NotAfter (Get-Date).AddYears(10) `
    -CertStoreLocation "cert:CurrentUser\My" `
    -FriendlyName "Localhost Certificate for .NET Core" `
    -HashAlgorithm SHA256 `
    -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") 
$certificatePath = 'Cert:\CurrentUser\My\' + ($certificate.ThumbPrint)  

# Create temporary certificate path
$tmpPath = "C:\Certs"
If(!(test-path $tmpPath))
{
New-Item -ItemType Directory -Force -Path $tmpPath
}

# Set certificate password here
$pfxPassword = ConvertTo-SecureString -String "@gBr@nd!2#4" -Force -AsPlainText
$pfxFilePath = "C:\Certs\agbrand-dev.pfx"
$cerFilePath = "C:\Certs\agbrand-dev.cer"

# Create pfx certificate
Export-PfxCertificate -Cert $certificatePath -FilePath $pfxFilePath -Password $pfxPassword
Export-Certificate -Cert $certificatePath -FilePath $cerFilePath