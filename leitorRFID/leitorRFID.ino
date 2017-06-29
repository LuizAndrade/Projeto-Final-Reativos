/*
  Programa: Yu-Gi-Oh! com leitor RFID
  Autores: Gustavo Silva
           Luiz Andrade
*/
#include <SPI.h>
#include <MFRC522.h>
#include <LiquidCrystal.h>
#include <SerialCommand.h>
#include <stdlib.h>

#define SS_PIN 10
#define RST_PIN 9

MFRC522 mfrc522(SS_PIN, RST_PIN);  // Cria instância MFRC522
LiquidCrystal lcd(6, 7, 5, 4, 3, 2); // Inicializa a biblioteca com os números dos pins da interface
SerialCommand sCmd;
unsigned long int count = 0;
bool fixed = false;

void setup() 
{
  Serial.begin(9600);   // Inicia a serial
  SPI.begin();      // Inicia  SPI bus
  mfrc522.PCD_Init();   // Inicia MFRC522
  lcd.begin(16, 2); //Define o número de colunas e linhas do LCD
  msgInicial();
}

void loop() 
{
    if (Serial.available() > 0)
    {
      sCmd.readSerial();
    }

    if(!fixed && millis()-count >= 3000) 
    {
      msgInicial();
      fixed = true;
    }
    
    if(!mfrc522.PICC_IsNewCardPresent()) { // Procura por novos cartões
        return;
    }
    
    if(!mfrc522.PICC_ReadCardSerial()) { // Aceita somente se um cartão for lido por vez
          return;
    }
  
    //Serial.print("UID da tag :"); Exibe UID na serial
    String conteudo = "";
    char conteudoChar[50];
    
    for(byte i = 0; i < mfrc522.uid.size; i++) 
    {
        conteudo.concat(String(mfrc522.uid.uidByte[i]));
    }
  
    if(validar(conteudo)) 
    {
        conteudo += '\n';
        conteudo.toCharArray(conteudoChar, 50);
        Serial.write(conteudoChar);
    }
    
    count = millis();
}

void msgInicial() 
{
  lcd.clear();
  lcd.print(" Aproxime o seu");  
  lcd.setCursor(0,1);
  lcd.print(" card, duelista");
}

bool validar(String conteudo)  
{
    lcd.clear();
    lcd.setCursor(0,0);
  
    if(conteudo == "2297012136") //UID 1 - Chaveiro - Dragao Branco
    { 
        lcd.print("ATK: 4500");
        lcd.setCursor(0,1);
        lcd.print("DEF: 3800");
        fixed = true;
        return true;
    }
    else if (conteudo == "1099148229") //UID 2 - Cartao - Mago Negro
    { 
        lcd.print("ATK: 2500");
        lcd.setCursor(0,1);
        lcd.print("DEF: 2100");
        fixed = true;
        return true;
    }
    
  //Cartões desconhecidos
  lcd.print("  Carta Falsa");
  lcd.setCursor(0,1);
  lcd.print("  OBLITERADO!");
  fixed = false;
  return false;
}
