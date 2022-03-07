//PC Monitor With Arduino and OLED 1,3" Screen
#include "ArduinoJson.h"
#include <U8g2lib.h>
#ifdef U8X8_HAVE_HW_SPI
#include <SPI.h>
#endif
#ifdef U8X8_HAVE_HW_I2C
#include <Wire.h>
#endif

U8G2_SH1106_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0, /* reset=*/ U8X8_PIN_NONE);
const byte numChars = 255 ;
char receivedChars[numChars];
boolean newData = false;
byte time;
StaticJsonDocument<200> doc;

//============

void setup() {
    Serial.begin(9600);
    u8g2.begin();
    start();
    //detecting();
}

//============

void loop() {
    getMsg();
    if (newData == true) {
        parseData();
        newData = false;
    }
}

//============

void getMsg() {
    static boolean recvInProgress = false;
    static byte ndx = 0;
    char startMarker = '<';
    char endMarker = '>';
    char rc;
    
    while (Serial.available() > 0 && newData == false) {
        rc = Serial.read();
        
        if (recvInProgress == true) {
            if (rc != endMarker) {
                receivedChars[ndx] = rc;
                ndx++;
                if (ndx >= numChars) {
                    ndx = numChars - 1;
                }
            }else {
                receivedChars[ndx] = '\0'; // terminate the string
                recvInProgress = false;
                ndx = 0;
                newData = true;
            }
        }else if (rc == startMarker) {
            recvInProgress = true;
        }
        time = 0;   
    }
    if(Serial.available() <= 0){   
        if(time >= 20){
            error();
        }else if(time > 5 && time < 20){
            detecting();
            delay(1000); 
        }
        time += 1;
   }
}

//============

void parseData() { 
   //delay(5000);
   //Serial.println(receivedChars);
    
    DeserializationError error = deserializeJson(doc, receivedChars);
     if (error) {
      Serial.print(F("deserializeJson() failed: "));
      Serial.println(error.f_str());
      u8g2.clearBuffer();
      u8g2.setFont(u8g2_font_ncenB08_tr);
      u8g2.drawStr( 50, 25,"Error");
      u8g2.drawStr( 15, 42,"deserializeJson()");
      u8g2.drawStr( 50, 57,"Failed");
      u8g2.sendBuffer();
    }
    bigScreen();
    //simpleScreen();
}

//============

//Oled Functions
//Show a welcome screen
void start(){
    u8g2.clearBuffer();
    u8g2.setFont(u8g2_font_ncenB08_tr);
    u8g2.drawStr(35, 25, "WELCOME");
    u8g2.drawStr(15, 45, "TO PC MONITOR");
    u8g2.setFont(u8g2_font_micro_tr);
    u8g2.drawStr(75, 58, "By Jarierca");
    u8g2.sendBuffer(); 
    delay(3000); 
}
//Show a detecting screen
void detecting(){
    u8g2.clearBuffer();
    u8g2.setFont(u8g2_font_ncenB08_tr);
    u8g2.drawStr(40, 25, "Detecting");
    u8g2.drawStr(60, 40, ".");
    u8g2.drawStr(60, 40, "..");
    u8g2.drawStr(60, 40, "...");
  
    u8g2.setFont(u8g2_font_micro_tr);
    u8g2.drawStr(75, 58, "By Jarierca");
    u8g2.sendBuffer(); 
   // delay(1500); 
}
//Show an error screen
void error(){
  u8g2.clearBuffer();
  u8g2.setFont(u8g2_font_ncenB08_tr);
  u8g2.drawStr(50, 25, "Error");
  u8g2.drawStr(25, 40, "No Data Input");
  u8g2.setFont(u8g2_font_micro_tr);
  u8g2.drawStr(75, 58, "By Jarierca");
  u8g2.sendBuffer(); 
  //delay(1500); 
}
//this method shows on a static screen the PC Data
void simpleScreen(){
    u8g2.clearBuffer();
    u8g2.setFont(u8g2_font_ncenB08_tr);
    //Title
    u8g2.drawStr( 30, 20, "PC MONITOR");
    u8g2.drawLine(0, 23, 127, 23);
  
    //CPU
    u8g2.drawStr( 5, 50, "CPU"); 
    u8g2.drawStr( 40, 40, doc["cpu"]["load"]); 
    u8g2.drawStr( 85, 40, doc["cpu"]["temp"]); 
    
    //GPU
    u8g2.drawStr( 5, 48, "GPU");
    u8g2.drawStr( 40, 48, doc["gpu"]["load"]);
    u8g2.drawStr( 85, 48, doc["gpu"]["temp"]);  
    
    //RAM
    u8g2.drawStr( 5, 61, "RAM");
    u8g2.drawStr( 40, 61, doc["ram"]["memTotal"]); 
    u8g2.drawStr( 83, 61, doc["ram"]["memUsed"]);

    u8g2.sendBuffer();
}

//this method shows on a changing screen the PC Data
void bigScreen(){
   
    //CPU
    u8g2.clearBuffer();
    u8g2.setFont(u8g2_font_helvR14_tr);
    u8g2.drawStr( 45, 25, "CPU");
    u8g2.setFont(u8g2_font_helvR10_tr);
    u8g2.drawStr( 15, 50, doc["cpu"]["load"]); 
    u8g2.drawStr( 80, 50, doc["cpu"]["temp"]); 
    u8g2.sendBuffer();
    delay(2500); 
   
    //GPU
    u8g2.clearBuffer();
    u8g2.setFont(u8g2_font_helvR14_tr);
    u8g2.drawStr( 45, 25, "GPU");
    u8g2.setFont(u8g2_font_helvR10_tr);
    u8g2.drawStr( 15, 42, doc["gpu"]["load"]); 
    u8g2.drawStr( 80, 42, doc["gpu"]["temp"]);  
    //u8g2.drawStr(85, 40,doc["gpu"]["clock"]);
    char gpuMem[19];
    strcpy(gpuMem, doc["gpu"]["memUsed"]);
    strcat(gpuMem, " / ");
    strcat(gpuMem, doc["gpu"]["memTotal"]);
    
    u8g2.drawStr( 12, 62, gpuMem);   
    u8g2.sendBuffer();
    delay(2500); 
    
    //RAM
    u8g2.clearBuffer();
    u8g2.setFont(u8g2_font_helvR14_tr);
    u8g2.drawStr( 45, 25, "RAM");
    u8g2.setFont(u8g2_font_helvR10_tr);
    u8g2.drawStr( 10, 50, doc["ram"]["memTotal"]); 
    u8g2.drawStr( 65, 50, doc["ram"]["memUsed"]); ; 
    u8g2.sendBuffer();
    delay(2500); 
}
