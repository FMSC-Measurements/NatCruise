

set keystorePath="../FScruiser.Droid/uploadKeystore.jks"

keytool -export -rfc -keystore %keystorePath% -alias upload -file upload_certificate.pem