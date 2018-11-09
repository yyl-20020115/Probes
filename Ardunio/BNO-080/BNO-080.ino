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

#include "SparkFun_BNO080_Arduino_Library.h"
BNO080 myIMU;

void setup()
{
  Serial.begin(115200);

  Wire.begin();
  Wire.setClock(400000); //Increase I2C data rate to 400kHz

  myIMU.begin();

  myIMU.enableRotationVector(50); //Send data update every 50ms
  myIMU.enableAccelerometer(50); //Send data update every 50ms
  myIMU.enableGyro(50); //Send data update every 50ms
  myIMU.enableMagnetometer(50); //Send data update every 50ms
  myIMU.enableStepCounter(50); //Send data update every 500ms

}

void loop()
{
  //Look for reports from the IMU
  if (myIMU.dataAvailable() == true)
  {
    float quatI = myIMU.getQuatI();
    float quatJ = myIMU.getQuatJ();
    float quatK = myIMU.getQuatK();
    float quatReal = myIMU.getQuatReal();
    float quatRadianAccuracy = myIMU.getQuatRadianAccuracy();
    float ax = myIMU.getAccelX();
    float ay = myIMU.getAccelY();
    float az = myIMU.getAccelZ();
    
    float gx = myIMU.getGyroX();
    float gy = myIMU.getGyroY();
    float gz = myIMU.getGyroZ();

    float mx = myIMU.getMagX();
    float my = myIMU.getMagY();
    float mz = myIMU.getMagZ();
    byte ma = myIMU.getMagAccuracy();

    unsigned int steps = myIMU.getStepCount();

    Serial.print(F("BNO:"));
    
    Serial.print(quatI, 2);
    Serial.print(F(","));
    Serial.print(quatJ, 2);
    Serial.print(F(","));
    Serial.print(quatK, 2);
    Serial.print(F(","));
    Serial.print(quatReal, 2);
    Serial.print(F(","));
    Serial.print(quatRadianAccuracy, 2);
    Serial.print(F(","));

    Serial.print(ax, 2);
    Serial.print(F(","));
    Serial.print(ay, 2);
    Serial.print(F(","));
    Serial.print(az, 2);
    Serial.print(F(","));

    
    Serial.print(gx, 2);
    Serial.print(F(","));
    Serial.print(gy, 2);
    Serial.print(F(","));
    Serial.print(gz, 2);
    Serial.print(F(","));


    Serial.print(mx, 2);
    Serial.print(F(","));
    Serial.print(my, 2);
    Serial.print(F(","));
    Serial.print(mz, 2);
    Serial.print(F(","));
    printAccuracyLevel(ma);
    Serial.print(F(","));
    
    Serial.print(steps);
    Serial.print(F(","));
    Serial.println();
  }
}


//Given a accuracy number, print what it means
void printAccuracyLevel(byte accuracyNumber)
{
  if(accuracyNumber == 0) Serial.print(F("Unreliable"));
  else if(accuracyNumber == 1) Serial.print(F("Low"));
  else if(accuracyNumber == 2) Serial.print(F("Medium"));
  else if(accuracyNumber == 3) Serial.print(F("High"));
}
