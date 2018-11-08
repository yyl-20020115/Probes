#include "MPU9150.h"
#include <stm32f4xx_rcc.h>
#include <stm32f4xx_gpio.h>
#include <stm32f4xx_i2c.h>
#include "stdint.h"

void MPU9150_Init() 
{	
	I2C_InitTypeDef  I2C_InitStructure;
  GPIO_InitTypeDef GPIO_InitStructure;

	RCC_AHB1PeriphClockCmd(MPU9150_I2C_RCC_Periph,ENABLE);

	RCC_AHB1PeriphClockCmd(MPU9150_I2C_RCC_Port_SCL | MPU9150_I2C_RCC_Port_SDA , ENABLE);  	

	GPIO_InitStructure.GPIO_Pin = MPU9150_I2C_SCL_Pin;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
	GPIO_InitStructure.GPIO_PuPd  =  GPIO_PuPd_NOPULL;;
	GPIO_Init(MPU9150_I2C_Port_SCL, &GPIO_InitStructure);

	GPIO_InitStructure.GPIO_Pin = MPU9150_I2C_SDA_Pin;
	GPIO_Init(MPU9150_I2C_Port_SDA, &GPIO_InitStructure);

	GPIO_PinAFConfig(MPU9150_I2C_Port_SCL, MPU9150_I2C_SCL_PinSource, MPU9150_I2C_AF);
	GPIO_PinAFConfig(MPU9150_I2C_Port_SDA, MPU9150_I2C_SDA_PinSource, MPU9150_I2C_AF);

	I2C_InitStructure.I2C_Mode = I2C_Mode_I2C;
	I2C_InitStructure.I2C_DutyCycle = I2C_DutyCycle_2;
	I2C_InitStructure.I2C_OwnAddress1 =0x0; // MPU9150 7-bit adress = 0x68, 8-bit adress = 0xD0; 
	I2C_InitStructure.I2C_Ack = I2C_Ack_Enable;
	I2C_InitStructure.I2C_AcknowledgedAddress = I2C_AcknowledgedAddress_7bit;
	I2C_InitStructure.I2C_ClockSpeed = MPU9150_I2C_Speed;
  
  /* Apply I2C configuration after enabling it */
  I2C_Init(MPU9150_I2C, &I2C_InitStructure);
  /* I2C Peripheral Enable */  
  I2C_Cmd(MPU9150_I2C, ENABLE);
	
	
	 MPU9150_I2C_ByteWrite(0xd0,0x00,MPU9150_RA_PWR_MGMT_1);
	 MPU9150_I2C_ByteWrite(0xd0,0x07,MPU9150_RA_SMPLRT_DIV);
	 MPU9150_I2C_ByteWrite(0xd0,0x06,MPU9150_RA_CONFIG);
	 MPU9150_I2C_ByteWrite(0xd0,0x01,MPU9150_RA_ACCEL_CONFIG);
	 MPU9150_I2C_ByteWrite(0xd0,0x18,MPU9150_RA_GYRO_CONFIG);
	 
}



void MPU9150_GetRawAccelGryo(short* AccelGyro) 
{
    unsigned char tmpBuffer[14],i; 
    MPU9150_I2C_BufferRead(0xd0, tmpBuffer, MPU9150_RA_ACCEL_XOUT_H, 14); 
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
    MPU9150_I2C_BufferRead(0xd0, tmpBuffer, MPU9150_RA_MAG_HXL, 6); 
    /* Get acceleration */
    for(i=0; i<3; i++) 
      Mag[i]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);
}

void MPU9150_GetRawData(short* Data) //9 shorts, 18 bytes
{
    unsigned char tmpBuffer[20],i; 
    MPU9150_I2C_BufferRead(0xd0, tmpBuffer, MPU9150_RA_ACCEL_XOUT_H, 14); 
    /* Get acceleration */
    for(i=0; i<3; i++) 
      Data[i]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);
   /* Get Angular rate */
    for(i=4; i<7; i++)
      Data[i-1]=((short)((unsigned short)tmpBuffer[2*i] << 8) + tmpBuffer[2*i+1]);        

    MPU9150_I2C_BufferRead(0xd0, tmpBuffer + 14, MPU9150_RA_MAG_HXL, 6); 
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
//  ENTR_CRT_SECTION();

  /* Send START condition */
  I2C_GenerateSTART(MPU9150_I2C, ENABLE);

  /* Test on EV5 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_MODE_SELECT));

  /* Send MPU9150 address for write */
  I2C_Send7bitAddress(MPU9150_I2C, slaveAddr, I2C_Direction_Transmitter);

  /* Test on EV6 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED));

  /* Send the MPU9150's internal address to write to */
  I2C_SendData(MPU9150_I2C, writeAddr);

  /* Test on EV8 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_BYTE_TRANSMITTED));

  /* Send the byte to be written */
  I2C_SendData(MPU9150_I2C, pBuffer);

  /* Test on EV8 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_BYTE_TRANSMITTED));

  /* Send STOP condition */
  I2C_GenerateSTOP(MPU9150_I2C, ENABLE);

 // EXT_CRT_SECTION();

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
 // ENTR_CRT_SECTION();

  /* While the bus is busy */
  
  while(I2C_GetFlagStatus(MPU9150_I2C, I2C_FLAG_BUSY));

  /* Send START condition */
  I2C_GenerateSTART(MPU9150_I2C, ENABLE);

  /* Test on EV5 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_MODE_SELECT));

  /* Send MPU9150 address for write */
  I2C_Send7bitAddress(MPU9150_I2C, slaveAddr, I2C_Direction_Transmitter); 

  /* Test on EV6 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED));

  /* Clear EV6 by setting again the PE bit */
  I2C_Cmd(MPU9150_I2C, ENABLE);

  /* Send the MPU9150's internal address to write to */
  I2C_SendData(MPU9150_I2C, readAddr);

  /* Test on EV8 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_BYTE_TRANSMITTED));

  /* Send STRAT condition a second time */
  I2C_GenerateSTART(MPU9150_I2C, ENABLE);

  /* Test on EV5 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_MODE_SELECT));

  /* Send MPU9150 address for read */
  I2C_Send7bitAddress(MPU9150_I2C, slaveAddr, I2C_Direction_Receiver);

  /* Test on EV6 and clear it */
  while(!I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_RECEIVER_MODE_SELECTED));

  /* While there is data to be read */
  while(NumByteToRead)
  {
    if(NumByteToRead == 1)
    {
      /* Disable Acknowledgement */
      I2C_AcknowledgeConfig(MPU9150_I2C, DISABLE);

      /* Send STOP Condition */
      I2C_GenerateSTOP(MPU9150_I2C, ENABLE);
    }

    /* Test on EV7 and clear it */
    if(I2C_CheckEvent(MPU9150_I2C, I2C_EVENT_MASTER_BYTE_RECEIVED))
    {
      /* Read a byte from the MPU9150 */
      *pBuffer = I2C_ReceiveData(MPU9150_I2C);

      /* Point to the next location where the byte read will be saved */
      pBuffer++;

      /* Decrement the read bytes counter */
      NumByteToRead--;
    }
  }

  /* Enable Acknowledgement to be ready for another reception */
  I2C_AcknowledgeConfig(MPU9150_I2C, ENABLE);
//  EXT_CRT_SECTION();

}
