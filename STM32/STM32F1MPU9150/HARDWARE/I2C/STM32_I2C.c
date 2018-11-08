#include "STM32_I2C.h"
#include "delay.h"
#include "led.h"


#define SCL_H          GPIO_SetBits(GPIOB , GPIO_Pin_6)   
#define SCL_L          GPIO_ResetBits(GPIOB , GPIO_Pin_6) 

#define SDA_H          GPIO_SetBits(GPIOB , GPIO_Pin_7)  
#define SDA_L          GPIO_ResetBits(GPIOB , GPIO_Pin_7) 

#define SCL_read       GPIO_ReadInputDataBit(GPIOB , GPIO_Pin_6) 
#define SDA_read       GPIO_ReadInputDataBit(GPIOB , GPIO_Pin_7) 


void I2C_delay(void)
{
    volatile int i = 7;
    while (i)
        i--;
}

void delay5ms(void)
{
   int i=5000;  
   while(i) 
   { 
     i--; 
   }  
}
bool I2C_Start(void)
{
    SDA_H;
    SCL_H;
    I2C_delay();
    if (!SDA_read)
        return false;
    SDA_L;
    I2C_delay();
    if (SDA_read)
        return false;
    SDA_L;
    I2C_delay();
    return true;
}

void I2C_Stop(void)
{
    SCL_L;
    I2C_delay();
    SDA_L;
    I2C_delay();
    SCL_H;
    I2C_delay();
    SDA_H;
    I2C_delay();
}

void I2C_Ack(void)
{
    SCL_L;
    I2C_delay();
    SDA_L;
    I2C_delay();
    SCL_H;
    I2C_delay();
    SCL_L;
    I2C_delay();
}

void I2C_NoAck(void)
{
    SCL_L;
    I2C_delay();
    SDA_H;
    I2C_delay();
    SCL_H;
    I2C_delay();
    SCL_L;
    I2C_delay();
}

bool I2C_WaitAck(void)
{
    SCL_L;
    I2C_delay();
    SDA_H;
    I2C_delay();
    SCL_H;
    I2C_delay();
    if (SDA_read) {
        SCL_L;
        return false;
    }
    SCL_L;
    return true;
}

void I2C_SendByte(uint8_t byte)
{
    uint8_t i = 8;
    while (i--) {
        SCL_L;
        I2C_delay();
        if (byte & 0x80)
            SDA_H;
        else
            SDA_L;
        byte <<= 1;
        I2C_delay();
        SCL_H;
        I2C_delay();
    }
    SCL_L;
}

uint8_t I2C_ReceiveByte(void)
{
    uint8_t i = 8;
    uint8_t byte = 0;

    SDA_H;
    while (i--) {
        byte <<= 1;
        SCL_L;
        I2C_delay();
        SCL_H;
        I2C_delay();
        if (SDA_read) {
            byte |= 0x01;
        }
    }
    SCL_L;
    return byte;
}

bool IIC_WriteBuffer(uint8_t addr, uint8_t reg, uint8_t len, uint8_t * data)
{
    int i;
    if (!I2C_Start())
        return false;
    I2C_SendByte(addr << 1 | I2C_Direction_Transmitter);
    if (!I2C_WaitAck()) {
        I2C_Stop();
        return false;
    }
    I2C_SendByte(reg);
    I2C_WaitAck();
    for (i = 0; i < len; i++) {
        I2C_SendByte(data[i]);
        if (!I2C_WaitAck()) {
            I2C_Stop();
            return false;
        }
    }
    I2C_Stop();
    return true;
}  
/////////////////////////////////////////////////////////////////////////////////
int8_t IIC_write(uint8_t addr, uint8_t reg, uint8_t len, uint8_t * data)
{
	if(IIC_WriteBuffer(addr,reg,len,data))
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
	//return FALSE;
} 
int8_t i2cread(uint8_t addr, uint8_t reg, uint8_t len, uint8_t *buf)
{
	if(I2cRead(addr,reg,len,buf))
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
	//return FALSE;
}  
bool I2cRead(uint8_t addr, uint8_t reg, uint8_t len, uint8_t *buf)
{
    if (!I2C_Start())
        return false;
    I2C_SendByte(addr << 1 | I2C_Direction_Transmitter);
    if (!I2C_WaitAck()) {
        I2C_Stop();
        return false;
    }
    I2C_SendByte(reg);
    I2C_WaitAck();
    I2C_Start();
    I2C_SendByte(addr << 1 | I2C_Direction_Receiver);
    I2C_WaitAck();
    while (len) {
        *buf = I2C_ReceiveByte();
        if (len == 1)
            I2C_NoAck();
        else
            I2C_Ack();
        buf++;
        len--;
    }
    I2C_Stop();
    return true;
}
//////////////////////////////////////////////////////////////////////////////////
unsigned char Single_Write (unsigned char SlaveAddress, unsigned char REG_Address, unsigned char REG_data)
{
    if (!I2C_Start())
        return false;
    I2C_SendByte(SlaveAddress<< 1 | I2C_Direction_Transmitter); //
    if (!I2C_WaitAck()) {
        I2C_Stop();
        return false;
    }
    I2C_SendByte(REG_Address);
    I2C_WaitAck();
    I2C_SendByte(REG_data);
    I2C_WaitAck();
    I2C_Stop();
    return true;
}

unsigned char IICn_Read(unsigned char SlaveAddress,unsigned char REG_Address)
{
    unsigned char REG_data;  
    if (!I2C_Start())
        return false;
    I2C_SendByte(SlaveAddress << 1 | I2C_Direction_Transmitter);
    if (!I2C_WaitAck()) {
        I2C_Stop();
        return false;
    }
    I2C_SendByte(REG_Address);
    I2C_WaitAck();
    I2C_Start();
    I2C_SendByte(SlaveAddress << 1 | I2C_Direction_Receiver);
    I2C_WaitAck();
    REG_data = I2C_ReceiveByte();
    I2C_NoAck(); 
    I2C_Stop();
    return REG_data;;
}

  
void Init_MPU9150(void)
{

  Single_Write(MPU9150_Addr,PWR_MGMT_1, 0x00);	//解除休眠状态
	delay5ms();
	Single_Write(MPU9150_Addr,SMPLRT_DIV, 0x07);	 //SMPLRT_DIV=0x19
	delay5ms();
	Single_Write(MPU9150_Addr,CONFIG, 0x06);		 ///CONFIG	低通滤波频率，典型值：0x06(5Hz)
	delay5ms();
	Single_Write(MPU9150_Addr,GYRO_CONFIG, 0x18);	 //GYRO-CFG（陀螺仪配置） +-2000d/s
	delay5ms();
	Single_Write(MPU9150_Addr,ACCEL_CONFIG, 0x01);	 //ACCEL（加速度计配置）
	delay5ms();
	
}
unsigned short GetData(unsigned char REG_Address)
{
	unsigned char H,L;
	H=IICn_Read(MPU9150_Addr,REG_Address);
	L=IICn_Read(MPU9150_Addr,REG_Address+1);
	return (H<<8)+L;   //合成数据
}

