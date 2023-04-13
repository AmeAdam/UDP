#https://deliciousbrains.com/ssl-certificate-authority-for-local-https-development/

#Generowanieklucza prywatnego dla CA
openssl genrsa -des3 -out AmeCA.key 2048

#Generowanie certyfikatu CA
openssl req -x509 -new -nodes -key AmeCA.key -sha256 -days 1825 -out AmeCA.pem

#Instalacja certyfikatu CA, 
#windows: certlm.msc
#linux: skopiuj AmeCA.crt /usr/local/share/ca-certificates

#Generowanie certyfikatu SSL:
#utworzenia klucza prywatnego
openssl genrsa -out ame.key 2048
#utworzenie csr
openssl req -new -key ame.key -out ame.csr
#wygeneruj plik konfiguracyjny dla certyfiaktu ame.ext
#[alt_names]
#DNS.1 = ame

#Generowanie certyfiaktu
openssl x509 -req -in ame.csr -CA AmeCA.pem -CAkey AmeCA.key -CAcreateserial -out ame.crt -days 825 -sha256 -extfile ame.ext



#PEM to CRT
openssl x509 -outform der -in AmeCA.pem -out AmeCA.crt


#To pfx no password
openssl pkcs12 -export -nodes -out ame.pfx -inkey ame.key -in ame.crt -certfile AmeCA.crt -passout pass: