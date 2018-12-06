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
String password = "Rabbit~Lion~20120115";
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
     Serial.println("Retry connecting host");
  }
  Serial.println("Host Connected");
  
  Wire.begin();
  Wire.setClock(400000); //Increase I2C data rate to 400kHz

  myIMU.begin();

  myIMU.enableRotationVector(50); //Send data update every 50ms
  myIMU.enableAccelerometer(50); //Send data update every 50ms
  myIMU.enableGyro(50); //Send data update every 50ms
  myIMU.enableMagnetometer(50); //Send data update every 50ms
}



String uint16ToHexString(uint16_t i)
{
  char buffer[5]={0};
  for(int t = 3;t>=0;t--)
  {
    int r = i & 0x0f;
    if(r>=0 && r<=9)
    {
      buffer[t] = '0'+r;
    }
    else{
      buffer[t] = 'A'+(r-10);
    }
    i>>=4;
  }
  return String(buffer);
}
void loop()
{
    if (myIMU.dataAvailable() == true)
    {

  //int16_t rotationVector_Q1 = 14;
  //int16_t accelerometer_Q1 = 8;
  //int16_t linear_accelerometer_Q1 = 8;
  //int16_t gyro_Q1 = 9;
  //int16_t magnetometer_Q1 = 4;
  
    
    uint16_t quatI = myIMU.getRawQuatI();
    uint16_t quatJ = myIMU.getRawQuatJ();
    uint16_t quatK = myIMU.getRawQuatK();
    uint16_t quatR = myIMU.getRawQuatReal();
    uint16_t quatA = myIMU.getRawQuatRadianAccuracy();
    uint16_t ax = myIMU.getRawAccelX();
    uint16_t ay = myIMU.getRawAccelY();
    uint16_t az = myIMU.getRawAccelZ();
    uint16_t aa = myIMU.getAccelAccuracy();

    uint16_t lx = myIMU.getRawLinAccelX();
    uint16_t ly = myIMU.getRawLinAccelY();
    uint16_t lz = myIMU.getRawLinAccelZ();

    uint16_t gx = myIMU.getRawGyroX();
    uint16_t gy = myIMU.getRawGyroY();
    uint16_t gz = myIMU.getRawGyroZ();
    uint16_t ga = myIMU.getGyroAccuracy();

    uint16_t mx = myIMU.getRawMagX();
    uint16_t my = myIMU.getRawMagY();
    uint16_t mz = myIMU.getRawMagZ();
    uint16_t ma = myIMU.getMagAccuracy();

    String line =  String("BNO:") 
          + uint16ToHexString(quatI) 
          + String(",") 
          + uint16ToHexString(quatJ)
          + String(",") 
          + uint16ToHexString(quatK) 
          + String(",") 
          + uint16ToHexString(quatR) 
          + String(",") 
          + uint16ToHexString(quatA) 
          + String(",")
          + uint16ToHexString(ax) 
          + String(",")
          + uint16ToHexString(ay) 
          + String(",")
          + uint16ToHexString(az)
          + String(",")
          + uint16ToHexString(aa)
          + String(",")
          + uint16ToHexString(lx)
          + String(",")
          + uint16ToHexString(ly)
          + String(",") 
          + uint16ToHexString(lz)
          + String(",")         
          + uint16ToHexString(gx) 
          + String(",")
          + uint16ToHexString(gy) 
          + String(",")
          + uint16ToHexString(gz) 
          + String(",") 
          + uint16ToHexString(ga) 
          + String(",") 
          + uint16ToHexString(mx) 
          + String(",")
          + uint16ToHexString(my)
          + String(",")
          + uint16ToHexString(mz) 
          + String(",") 
          + uint16ToHexString(ma);
        client.println(
          line
          );
        Serial.println(line);
  }
}
