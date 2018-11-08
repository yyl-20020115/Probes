#include "MPU9150.h"
#include <stm32f4xx_rcc.h>
#include <stm32f4xx_gpio.h>
#include <stm32f4xx_i2c.h>
#include "stdint.h"
#include "softi2c.h"

SOFTI2C si2c={0};

void MPU9150_Init() 
{	

	RCC_AHB1PeriphClockCmd(MPU9150_I2C_RCC_Port_SCL | MPU9150_I2C_RCC_Port_SDA , ENABLE);  	

	si2c.ID=1;
	si2c.SCL_Port = MPU9150_I2C_Port_SCL;
	si2c.SDA_Port = MPU9150_I2C_Port_SDA;
	si2c.SCL_Pin = MPU9150_I2C_SCL_Pin;
	si2c.SDA_Pin = MPU9150_I2C_SDA_Pin;
	si2c.DelayValue = 100;

	SOFTI2C_Init(&si2c);
	
	MPU9150_I2C_ByteWrite(MPU9150_SLAVE_ADDRESS,0x00,MPU9150_RA_PWR_MGMT_1);
	MPU9150_I2C_ByteWrite(MPU9150_SLAVE_ADDRESS,0x07,MPU9150_RA_SMPLRT_DIV);
	MPU9150_I2C_ByteWrite(MPU9150_SLAVE_ADDRESS,0x06,MPU9150_RA_CONFIG);
	MPU9150_I2C_ByteWrite(MPU9150_SLAVE_ADDRESS,0x18,MPU9150_RA_GYRO_CONFIG);
	MPU9150_I2C_ByteWrite(MPU9150_SLAVE_ADDRESS,0x01,MPU9150_RA_ACCEL_CONFIG);
	 
}

unsigned char MPU9150_Read(unsigned char slaveAddr,unsigned char readAddr)
{
		unsigned char data = 0;
	
	  if (SOFTI2C_START(&si2c) != SOFTI2C_READY)
        return data;
    SOFTI2C_SendByte(&si2c,slaveAddr << 1 | I2C_Direction_Transmitter);
    if (!SOFTI2C_WaitACK(&si2c)) {
        SOFTI2C_STOP(&si2c);
        return data;
    }
    SOFTI2C_SendByte(&si2c,readAddr);
    SOFTI2C_WaitACK(&si2c);
    SOFTI2C_START(&si2c);
    SOFTI2C_SendByte(&si2c,slaveAddr << 1 | I2C_Direction_Receiver);
    SOFTI2C_WaitACK(&si2c);

    data = SOFTI2C_ReceiveByte(&si2c);
	  SOFTI2C_SendNACK(&si2c); 
	  SOFTI2C_STOP(&si2c);
    return data;
}
unsigned short MPU9150_GetData(unsigned char slaveAddr,unsigned char readAddr)
{
	unsigned char H,L;
	H=MPU9150_Read(slaveAddr,readAddr);
	L=MPU9150_Read(slaveAddr,readAddr+1);
	return ((unsigned short)(H<<8))|L;   //合成数据
}
void MPU9150_GetRawAccel(short* Accel)
{
		*Accel = MPU9150_GetData(MPU9150_SLAVE_ADDRESS,MPU9150_RA_ACCEL_XOUT_H);
}	

void MPU9150_GetRawAccelGryo(short* AccelGyro) 
{
    unsigned char tmpBuffer[14],i; 
    MPU9150_I2C_BufferRead(MPU9150_SLAVE_ADDRESS, tmpBuffer, MPU9150_RA_ACCEL_XOUT_H, 14); 
    /* Get acceleration */
    for(i=0; i<3; i++) 
      AccelGyro[i]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);
   /* Get Angular rate */
    for(i=4; i<7; i++)
      AccelGyro[i-1]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);        

}

void MPU9150_GetRawMagento(short* Mag) 
{
    unsigned char tmpBuffer[6],i; 
    MPU9150_I2C_BufferRead(MPU9150_SLAVE_ADDRESS, tmpBuffer, MPU9150_RA_MAG_HXL, 6); 
    /* Get acceleration */
    for(i=0; i<3; i++) 
      Mag[i]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);
}

void MPU9150_GetRawData(short* Data) //9 shorts, 18 bytes
{
    unsigned char tmpBuffer[20],i; 
    MPU9150_I2C_BufferRead(MPU9150_SLAVE_ADDRESS, tmpBuffer, MPU9150_RA_ACCEL_XOUT_H, 14); 
    /* Get acceleration */
    for(i=0; i<3; i++) 
      Data[i]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);
   /* Get Angular rate */
    for(i=4; i<7; i++)
      Data[i-1]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);        

    MPU9150_I2C_BufferRead(MPU9150_SLAVE_ADDRESS, tmpBuffer + 14, MPU9150_RA_MAG_HXL, 6); 
    /* Get acceleration */
    for(i=7; i<10; i++) 
      Data[i]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);
}

/**
* @brief  Writes one byte to the  MPU9150.
* @param  slaveAddr : slave address MPU9150_DEFAULT_ADDRESS
* @param  pBuffer : pointer to the buffer  containing the data to be written to the MPU9150.
* @param  writeAddr : address of the register in which the data will be written
* @return None
*/
void MPU9150_I2C_ByteWrite(unsigned char slaveAddr, unsigned char pBuffer, unsigned char writeAddr)
{
    if (SOFTI2C_START(&si2c) != SOFTI2C_READY)
        return;
		
    SOFTI2C_SendByte(&si2c,slaveAddr<< 1 | I2C_Direction_Transmitter); //
    if (!SOFTI2C_WaitACK(&si2c)) {
        SOFTI2C_STOP(&si2c);
        return;
    }
    SOFTI2C_SendByte(&si2c,writeAddr);
    SOFTI2C_WaitACK(&si2c);
    SOFTI2C_SendByte(&si2c,pBuffer);
    SOFTI2C_WaitACK(&si2c);
    SOFTI2C_STOP(&si2c);
}

/**
* @brief  Reads a block of data from the MPU9150.
* @param  slaveAddr  : slave address MPU9150_DEFAULT_ADDRESS
* @param  pBuffer : pointer to the buffer that receives the data read from the MPU9150.
* @param  readAddr : MPU9150's internal address to read from.
* @param  NumByteToRead : number of bytes to read from the MPU9150 ( NumByteToRead >1  only for the Mgnetometer readinf).
* @return None
*/

void MPU9150_I2C_BufferRead(unsigned char slaveAddr, unsigned char* pBuffer, unsigned char readAddr, unsigned short NumByteToRead)
{
    if (SOFTI2C_START(&si2c) != SOFTI2C_READY)
        return;
    SOFTI2C_SendByte(&si2c,slaveAddr << 1 | I2C_Direction_Transmitter);
    if (!SOFTI2C_WaitACK(&si2c)) {
        SOFTI2C_STOP(&si2c);
        return;
    }
    SOFTI2C_SendByte(&si2c,readAddr);
    SOFTI2C_WaitACK(&si2c);
    SOFTI2C_START(&si2c);
    SOFTI2C_SendByte(&si2c,slaveAddr << 1 | I2C_Direction_Receiver);
    SOFTI2C_WaitACK(&si2c);
		
  /* While there is data to be read */
  while(NumByteToRead)
  {
		if(SOFTI2C_WaitACK(&si2c))
    {
      /* Read a byte from the MPU9150 */
      *pBuffer = SOFTI2C_ReceiveByte(&si2c);

      /* Point to the next location where the byte read will be saved */
      pBuffer++;

      /* Decrement the read bytes counter */
      NumByteToRead--;
    }
  }
	
	SOFTI2C_SendNACK(&si2c); 
	SOFTI2C_STOP(&si2c);
}
