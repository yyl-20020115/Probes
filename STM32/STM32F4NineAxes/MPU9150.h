#ifndef __MPU9150_H__
#define __MPU9150_H__

//PA8: I2C3_SCL  
//PC9: I2C3_SDA
//NOTUSED:	 	PC8: INT
#define MPU9150_I2C                  I2C3
#define MPU9150_I2C_RCC_Periph       RCC_APB1Periph_I2C3
#define MPU9150_I2C_Port_SCL         GPIOA
#define MPU9150_I2C_Port_SDA         GPIOC
#define MPU9150_I2C_SCL_Pin          GPIO_Pin_8
#define MPU9150_I2C_SDA_Pin          GPIO_Pin_9
#define MPU9150_I2C_SCL_PinSource		 GPIO_PinSource8
#define MPU9150_I2C_SDA_PinSource		 GPIO_PinSource9
#define MPU9150_I2C_RCC_Port_SCL     RCC_AHB1Periph_GPIOA
#define MPU9150_I2C_RCC_Port_SDA     RCC_AHB1Periph_GPIOC
#define MPU9150_I2C_AF							 GPIO_AF_I2C3
#define MPU9150_I2C_Speed            100000 // 100kHz standard mode


#define MPU9150_RA_SMPLRT_DIV       0x19
#define MPU9150_RA_CONFIG           0x1A
#define MPU9150_RA_GYRO_CONFIG      0x1B 
#define MPU9150_RA_ACCEL_CONFIG     0x1C

#define MPU9150_RA_FIFO_EN          0x23
#define MPU9150_RA_I2C_MST_CTRL     0x24
#define MPU9150_RA_I2C_SLV0_ADDR    0x25
#define MPU9150_RA_I2C_SLV0_REG     0x26
#define MPU9150_RA_I2C_SLV0_CTRL    0x27
#define MPU9150_RA_I2C_SLV1_ADDR    0x28
#define MPU9150_RA_I2C_SLV1_REG     0x29
#define MPU9150_RA_I2C_SLV1_CTRL    0x2A
#define MPU9150_RA_I2C_SLV2_ADDR    0x2B
#define MPU9150_RA_I2C_SLV2_REG     0x2C
#define MPU9150_RA_I2C_SLV2_CTRL    0x2D
#define MPU9150_RA_I2C_SLV3_ADDR    0x2E
#define MPU9150_RA_I2C_SLV3_REG     0x2F
#define MPU9150_RA_I2C_SLV3_CTRL    0x30
#define MPU9150_RA_I2C_SLV4_ADDR    0x31
#define MPU9150_RA_I2C_SLV4_REG     0x32
#define MPU9150_RA_I2C_SLV4_DO      0x33
#define MPU9150_RA_I2C_SLV4_CTRL    0x34
#define MPU9150_RA_I2C_SLV4_DI      0x35

#define MPU9150_RA_I2C_MST_STATUS   0x36
#define MPU9150_RA_INT_PIN_CFG      0x37
#define MPU9150_RA_INT_ENABLE       0x38
#define MPU9150_RA_DMP_INT_STATUS   0x39
#define MPU9150_RA_INT_STATUS       0x3A
#define MPU9150_RA_ACCEL_XOUT_H     0x3B
#define MPU9150_RA_ACCEL_XOUT_L     0x3C
#define MPU9150_RA_ACCEL_YOUT_H     0x3D
#define MPU9150_RA_ACCEL_YOUT_L     0x3E
#define MPU9150_RA_ACCEL_ZOUT_H     0x3F
#define MPU9150_RA_ACCEL_ZOUT_L     0x40
#define MPU9150_RA_TEMP_OUT_H       0x41
#define MPU9150_RA_TEMP_OUT_L       0x42
#define MPU9150_RA_GYRO_XOUT_H      0x43
#define MPU9150_RA_GYRO_XOUT_L      0x44
#define MPU9150_RA_GYRO_YOUT_H      0x45
#define MPU9150_RA_GYRO_YOUT_L      0x46
#define MPU9150_RA_GYRO_ZOUT_H      0x47
#define MPU9150_RA_GYRO_ZOUT_L      0x48
#define MPU9150_RA_EXT_SENS_DATA_00 0x49
#define MPU9150_RA_EXT_SENS_DATA_01 0x4A
#define MPU9150_RA_EXT_SENS_DATA_02 0x4B
#define MPU9150_RA_EXT_SENS_DATA_03 0x4C
#define MPU9150_RA_EXT_SENS_DATA_04 0x4D
#define MPU9150_RA_EXT_SENS_DATA_05 0x4E
#define MPU9150_RA_EXT_SENS_DATA_06 0x4F
#define MPU9150_RA_EXT_SENS_DATA_07 0x50
#define MPU9150_RA_EXT_SENS_DATA_08 0x51
#define MPU9150_RA_EXT_SENS_DATA_09 0x52
#define MPU9150_RA_EXT_SENS_DATA_10 0x53
#define MPU9150_RA_EXT_SENS_DATA_11 0x54
#define MPU9150_RA_EXT_SENS_DATA_12 0x55
#define MPU9150_RA_EXT_SENS_DATA_13 0x56
#define MPU9150_RA_EXT_SENS_DATA_14 0x57
#define MPU9150_RA_EXT_SENS_DATA_15 0x58
#define MPU9150_RA_EXT_SENS_DATA_16 0x59
#define MPU9150_RA_EXT_SENS_DATA_17 0x5A
#define MPU9150_RA_EXT_SENS_DATA_18 0x5B
#define MPU9150_RA_EXT_SENS_DATA_19 0x5C
#define MPU9150_RA_EXT_SENS_DATA_20 0x5D
#define MPU9150_RA_EXT_SENS_DATA_21 0x5E
#define MPU9150_RA_EXT_SENS_DATA_22 0x5F
#define MPU9150_RA_EXT_SENS_DATA_23 0x60
#define MPU9150_RA_MOT_DETECT_STATUS    0x61
#define MPU9150_RA_I2C_SLV0_DO      0x63
#define MPU9150_RA_I2C_SLV1_DO      0x64
#define MPU9150_RA_I2C_SLV2_DO      0x65
#define MPU9150_RA_I2C_SLV3_DO      0x66
#define MPU9150_RA_I2C_MST_DELAY_CTRL   0x67
#define MPU9150_RA_SIGNAL_PATH_RESET    0x68
#define MPU9150_RA_MOT_DETECT_CTRL      0x69
#define MPU9150_RA_USER_CTRL        0x6A
#define MPU9150_RA_PWR_MGMT_1       0x6B
#define MPU9150_RA_PWR_MGMT_2       0x6C
#define MPU9150_RA_BANK_SEL         0x6D
#define MPU9150_RA_MEM_START_ADDR   0x6E
#define MPU9150_RA_MEM_R_W          0x6F
#define MPU9150_RA_DMP_CFG_1        0x70
#define MPU9150_RA_DMP_CFG_2        0x71
#define MPU9150_RA_FIFO_COUNTH      0x72
#define MPU9150_RA_FIFO_COUNTL      0x73
#define MPU9150_RA_FIFO_R_W         0x74
#define MPU9150_RA_WHO_AM_I         0x75 


#define MPU9150_RA_MAG_WIA          0x00 
#define MPU9150_RA_MAG_INFO         0x01 
#define MPU9150_RA_MAG_ST1          0x02 
#define MPU9150_RA_MAG_HXL          0x03 
#define MPU9150_RA_MAG_HXH          0x04 
#define MPU9150_RA_MAG_HYL          0x05 
#define MPU9150_RA_MAG_HYH          0x06 
#define MPU9150_RA_MAG_HZL          0x07 
#define MPU9150_RA_MAG_HZH          0x08 
#define MPU9150_RA_MAG_ST2          0x09 
#define MPU9150_RA_MAG_CNTL         0x0A 
#define MPU9150_RA_MAG_RSV          0x0B 
#define MPU9150_RA_MAG_ASTC         0x0C 
#define MPU9150_RA_MAG_TS1          0x0D 
#define MPU9150_RA_MAG_TS2          0x0E 
#define MPU9150_RA_MAG_I2CDIS       0x0F 

#define MPU9150_RA_MAG_ASAX         0x10 
#define MPU9150_RA_MAG_ASAY         0x11 
#define MPU9150_RA_MAG_ASAZ         0x12 


void MPU9150_Init(void);


void MPU9150_GetRawAccelGyro(short* AccelGyro);
void MPU9150_GetRawMagento(short* Mag);
void MPU9150_GetRawData(short* Data); //9 shorts, 18 bytes
	
void MPU9150_I2C_ByteWrite(unsigned char slaveAddr, unsigned char pBuffer, unsigned char writeAddr);
void MPU9150_I2C_BufferRead(unsigned char slaveAddr,unsigned char* pBuffer, unsigned char readAddr, unsigned short NumByteToRead);


#endif
