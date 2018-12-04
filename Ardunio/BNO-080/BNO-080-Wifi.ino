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
char s01[] = "";
char s02[] = "";
char s03[] = "";
char s04[] = "";
char s05[] = "";
char s06[] = "";
char s07[] = "";
char s08[] = "";
char s09[] = "";
char s10[] = "";
char s11[] = "";
char s12[] = "";
char s13[] = "";
char s14[] = "";
void loop()
{
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


        dtostrf(quatI,w,p,s01);
        dtostrf(quatJ,w,p,s02);
        dtostrf(quatK,w,p,s03);
        dtostrf(quatR,w,p,s04);
        dtostrf(quatA,w,p,s05);
        dtostrf(ax,w,p,s06);
        dtostrf(ay,w,p,s07);
        dtostrf(az,w,p,s08);
        dtostrf(gx,w,p,s09);
        dtostrf(gy,w,p,s10);
        dtostrf(gz,w,p,s11);
        dtostrf(mx,w,p,s12);
        dtostrf(my,w,p,s13);
        dtostrf(mz,w,p,s14);

        //client.printf("BNO:%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%d\n", s01,s02,s03,s04,s05,s06,s07,s08,s09,s10,s11,s12,s13,s14,ma);
        //client.println("Hello");
        delay(10);

  }
}
