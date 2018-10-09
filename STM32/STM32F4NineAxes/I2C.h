#ifndef __I2C_H__
#define __I2C_H__

#include "stm32f4xx_i2c.h"
//I2C1: SCL=PB6,SDA=PB7
//I2C2: SCL=PB10,SDA=PB11
//I2C3: SCL=PA8,SDA=PC9

#define I2C_CLOCK_SPEED 100000

void I2C_Config(void);

unsigned char I2C_GetErrorCode(void);

unsigned char I2C_ReadOneByte(I2C_TypeDef *I2Cx,unsigned char I2C_Addr,unsigned char Reg_addr);
void I2C_WriteOneByte(I2C_TypeDef *I2Cx,uint8_t I2C_Addr,uint8_t Reg_addr,uint8_t value);

#endif
