#ifndef __STM32_I2C_H
#define __STM32_I2C_H
//#include "define.h"
#include "stm32f10x.h"

//typedef bool char;
//
//#define false    0
//#define	true     1

#define CLI()      __set_PRIMASK(1)  
#define SEI()      __set_PRIMASK(0)

#define BYTE0(dwTemp)       (*(char *)(&dwTemp))
#define BYTE1(dwTemp)       (*((char *)(&dwTemp) + 1))
#define BYTE2(dwTemp)       (*((char *)(&dwTemp) + 2))
#define BYTE3(dwTemp)       (*((char *)(&dwTemp) + 3))

#define true 1
#define false 0 
#define   bool  uint8_t

#define TRUE  0
#define FALSE -1
/*
#define MPU6050_READRATE			1000	//6050��ȡƵ��
#define MPU6050_READTIME			0.001	//6050��ȡʱ����
#define EE_6050_ACC_X_OFFSET_ADDR	0
#define EE_6050_ACC_Y_OFFSET_ADDR	1
#define EE_6050_ACC_Z_OFFSET_ADDR	2
#define EE_6050_GYRO_X_OFFSET_ADDR	3
#define EE_6050_GYRO_Y_OFFSET_ADDR	4
#define EE_6050_GYRO_Z_OFFSET_ADDR	5
*/
#define TAP_X               (0x01)
#define TAP_Y               (0x02)
#define TAP_Z               (0x04)
#define TAP_XYZ             (0x07)

#define TAP_X_UP            (0x01)
#define TAP_X_DOWN          (0x02)
#define TAP_Y_UP            (0x03)
#define TAP_Y_DOWN          (0x04)
#define TAP_Z_UP            (0x05)
#define TAP_Z_DOWN          (0x06)

#define ANDROID_ORIENT_PORTRAIT             (0x00)
#define ANDROID_ORIENT_LANDSCAPE            (0x01)
#define ANDROID_ORIENT_REVERSE_PORTRAIT     (0x02)
#define ANDROID_ORIENT_REVERSE_LANDSCAPE    (0x03)


/* MPU6050 Register Address ------------------------------------------------------------*/
#define	SMPLRT_DIV		0x19	//�����ǲ����ʣ�����ֵ��0x07(125Hz)
#define	CONFIG				0x1A	//��ͨ�˲�Ƶ�ʣ�����ֵ��0x06(5Hz)
#define	GYRO_CONFIG		0x1B	//�������Լ켰������Χ������ֵ��0x18(���Լ죬2000deg/s)
#define	ACCEL_CONFIG	0x1C	//���ټ��Լ졢������Χ����ͨ�˲�Ƶ�ʣ�����ֵ��0x01(���Լ죬2G��5Hz)
#define	ACCEL_XOUT_H	0x3B
#define	ACCEL_XOUT_L	0x3C
#define	ACCEL_YOUT_H	0x3D
#define	ACCEL_YOUT_L	0x3E
#define	ACCEL_ZOUT_H	0x3F
#define	ACCEL_ZOUT_L	0x40
#define	TEMP_OUT_H		0x41
#define	TEMP_OUT_L		0x42
#define	GYRO_XOUT_H		0x43
#define	GYRO_XOUT_L		0x44	
#define	GYRO_YOUT_H		0x45
#define	GYRO_YOUT_L		0x46
#define	GYRO_ZOUT_H		0x47
#define	GYRO_ZOUT_L		0x48
#define	PWR_MGMT_1		0x6B	//��Դ��������ֵ��0x00(��������)
#define	WHO_AM_I			0x75	//IIC��ַ�Ĵ���(Ĭ����ֵ0x68��ֻ��)
//#define	SlaveAddress	0xD0	//IICд��ʱ�ĵ�ַ�ֽ�����
#define	MPU9150_Addr   0x68	  //����������IIC�����еĴӵ�ַ,����ALT  ADDRESS��ַ���Ų�ͬ�޸�


void I2C_delay(void);
void Init_MPU9150(void);
unsigned short GetData(unsigned char REG_Address);
int8_t IIC_write(uint8_t addr, uint8_t reg, uint8_t len, uint8_t * data);
bool IIC_WriteBuffer(uint8_t addr, uint8_t reg, uint8_t len, uint8_t * data);
int8_t i2cread(uint8_t addr, uint8_t reg, uint8_t len, uint8_t *buf);
//float i2cWrite(unsigned char SlaveAddress, unsigned char REG_Address, unsigned char REG_data);
//float i2c_Read(unsigned char SlaveAddress,unsigned char REG_Address);
bool I2cRead(uint8_t addr, uint8_t reg, uint8_t len, uint8_t *buf);
unsigned char IICn_Read(unsigned char SlaveAddress,unsigned char REG_Address);
unsigned char Single_Write (unsigned char SlaveAddress, unsigned char REG_Address, unsigned char REG_data);
//bool i2cWriteBuffer(uint8_t addr, uint8_t reg, uint8_t len, uint8_t * data);
//int8_t i2cwrite(uint8_t addr, uint8_t reg, uint8_t len, uint8_t * data);
//int8_t i2cread(uint8_t addr, uint8_t reg, uint8_t len, uint8_t *buf);
//unsigned char  i2c_Read(unsigned char SlaveAddress,unsigned char REG_Address)


#endif
