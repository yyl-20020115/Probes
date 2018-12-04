/*
  Using the BNO080 IMU
  By: Nathan Seidle
  SparkFun Electronics
  Date: December 21st, 2017
  License: This code is public domain but you buy me a beer if you use this and we meet someday (Beerware license).

  Feel like supporting our work? Buy a board from SparkFun!
  https://www.sparkfun.com/products/14586

  This example shows how to output the i/j/k/real parts of the rotation vector.
  https://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation

  It takes about 1ms at 400kHz I2C to read a record from the sensor, but we are polling the sensor continually
  between updates from the sensor. Use the interrupt pin on the BNO080 breakout to avoid polling.

  Hardware Connections:
  Attach the Qwiic Shield to your Arduino/Photon/ESP32 or other
  Plug the sensor onto the shield
  Serial.print it out at 9600 baud to serial monitor.
*/

#include <Wire.h>
#include <ESP8266WiFi.h>
#include "SparkFun_BNO080_Arduino_Library.h"

String ssid = "YILIN-HOME"; 
String password = "";
String host = "192.168.1.88";
int port = 6000;

BNO080 myIMU;

WiFiClient client;

void setup()
{
  Serial.begin(115200);
  Serial.println("Started");
  WiFi.begin ( ssid.c_str(), password.c_str() );
  while ( WiFi.status() != WL_CONNECTED ) {
    delay ( 1000 );
     Serial.println("Retry connecting router");
  }

  Serial.println("Router Connected");
  while (!client.connect(host, port)) {
    delay(1000);
  }
  Serial.println("Client Connected");
  
  Wire.begin();
  Wire.setClock(400000); //Increase I2C data rate to 400kHz

  myIMU.begin();

  myIMU.enableRotationVector(50); //Send data update every 50ms
  myIMU.enableAccelerometer(50); //Send data update every 50ms
  myIMU.enableGyro(50); //Send data update every 50ms
  myIMU.enableMagnetometer(50); //Send data update every 50ms
}


String floatToString(float f){
  char buffer[12]="";
  int w = 4,p = 4;
  dtostrf(f,w,p,buffer);
  return String(buffer);
}
void loop()
{
    char itoabuffer[12]={0};
    
    int w = 4, p = 4;
    
    Serial.println("Looping");

    if (myIMU.dataAvailable() == true)
    {
      Serial.println("got data");
        float quatI = myIMU.getQuatI();
        float quatJ = myIMU.getQuatJ();
        float quatK = myIMU.getQuatK();
        float quatR = myIMU.getQuatReal();
        float quatA = myIMU.getQuatRadianAccuracy();
        float ax = myIMU.getAccelX();
        float ay = myIMU.getAccelY();
        float az = myIMU.getAccelZ();   
        float gx = myIMU.getGyroX();
        float gy = myIMU.getGyroY();
        float gz = myIMU.getGyroZ();
        float mx = myIMU.getMagX();
        float my = myIMU.getMagY();
        float mz = myIMU.getMagZ();
        int ma = myIMU.getMagAccuracy();

        client.println(
          String("BNO:") 
          + floatToString(quatI) 
          + String(",") 
          + floatToString(quatJ)
          + String(",") 
          + floatToString(quatK) 
          + String(",") 
          + floatToString(quatR) 
          + String(",") 
          + floatToString(quatA) 
          + String(",")
          + floatToString(ax) 
          + String(",")
          + floatToString(ay) 
          + String(",")
          + floatToString(az)
          + String(",")
          + floatToString(gx) 
          + String(",")
          + floatToString(gy) 
          + String(",")
          + floatToString(gz) 
          + String(",") 
          + floatToString(mx) 
          + String(",")
          + floatToString(my)
          + String(",")
          + floatToString(mz) 
          + String(",") 
          + String(itoa(ma,itoabuffer,10))
          );
        
        delay(10);

  }
}
