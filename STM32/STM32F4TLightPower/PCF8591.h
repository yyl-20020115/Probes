#ifndef __PCF8591_H__
#define __PCF8591_H__

//USE I2C3: SCL=PA8,SDA=PC9
#define PCF8591_I2C_CLOCK_SPEED         100000

#define PCF8591_RCC_SCL         RCC_AHB1Periph_GPIOA
#define PCF8591_RCC_SDA         RCC_AHB1Periph_GPIOC

#define PCF8591_I2C             I2C3
#define PCF8591_AF              GPIO_AF_I2C3
#define PCF8591_RCC             RCC_APB1Periph_I2C3

#define PCF8591_SCL_SRC_PIN     GPIO_PinSource8
#define PCF8591_SDA_SRC_PIN     GPIO_PinSource9

#define PCF8591_I2C_SCL_PORT    GPIOA
#define PCF8591_I2C_SCL_PIN     GPIO_Pin_8

#define PCF8591_I2C_SDA_PORT    GPIOC
#define PCF8591_I2C_SDA_PIN     GPIO_Pin_9


#define PCF8591_WRITE_ADDRESS   0x90   //写数据地址 
#define PCF8591_READ_ADDRESS    0x91   //读数据地址

void PCF8591_Init(void);
unsigned char PCF8591_GetErrorCode(void);
//8 bit AD
//CH0,1,3: HR,LR,R
unsigned char PCF8591_ReadADC(unsigned char Channel);
//8 bit DA
//255 = 4.096V(@5V)
void PCF8591_WriteDAC(unsigned char DAValue);

#endif
