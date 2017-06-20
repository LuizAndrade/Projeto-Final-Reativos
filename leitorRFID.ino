/*
  Programa: Yu-Gi-Oh! com leitor RFID
  Autores: Gustavo Silva
           Luiz Andrade
*/
#include <SPI.h>
#include <MFRC522.h>
#include <LiquidCrystal.h>
#include <SerialCommand.h>

#define SS_PIN 10
#define RST_PIN 9

MFRC522 mfrc522(SS_PIN, RST_PIN);  // Cria instância MFRC522
LiquidCrystal lcd(6, 7, 5, 4, 3, 2); // Inicializa a biblioteca com os números dos pins da interface
SerialCommand sCmd;

boolean reading = false;
unsigned long int count;

void setup() {
  Serial.begin(9600);   // Inicia a serial
  SPI.begin();      // Inicia  SPI bus
  mfrc522.PCD_Init();   // Inicia MFRC522
  lcd.begin(16, 2); //Define o número de colunas e linhas do LCD
  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  msgInicial();
}

void loop() {

  if (Serial.available() > 0){
    sCmd.readSerial();
  }
  
  if(reading && millis()-count >= 3000) {
    reading = false;
    msgInicial();
  }
  
  if(!mfrc522.PICC_IsNewCardPresent()) { // Procura por novos cartões
      return;
  }
  
  if(!mfrc522.PICC_ReadCardSerial()) { // Aceita somente se um cartão for lido por vez
        return;
    }
  
  reading = true;
  count = millis();
  
//  Serial.print("UID da tag :"); Exibe UID na serial
    String conteudo = "";
  
    for(byte i = 0; i < mfrc522.uid.size; i++) {
//        Serial.print(mfrc522.uid.uidByte[i]);
        conteudo.concat(String(mfrc522.uid.uidByte[i]));
    }
  
    //Serial.println();

  if(validar(conteudo)) {
        //rotacionarServo();
    }
}

void msgInicial() {
  lcd.clear();
  lcd.print(" Aproxime o seu");  
  lcd.setCursor(0,1);
  lcd.print("cartao do leitor");
}

bool validar(String conteudo)  {
    lcd.clear();
    lcd.setCursor(0,0);
  
    if(conteudo == "2297012136") { //UID 1 - Chaveiro
        lcd.print("Ola, professor!");
        lcd.setCursor(0,1);
        lcd.print("Acesso liberado!");
        return true;
  }
  //Cartões desconhecidos
  lcd.print("Ola, aluno!");
  lcd.setCursor(0,1);
  lcd.print("Acesso Negado!");
  return false;
}

void pingHandler(){
  Serial.println("PONG");
}

void echoHandler () {
  char *arg;
  arg = sCmd.next();
  if (arg != NULL){
    Serial.println(arg);
  }
  else{
    Serial.println("nothing to echo");
  }
}
