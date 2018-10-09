#ifndef __MCP3221_H__
#define __MCP3221_H__
//USE I2C2: SCL=PB10,SDA=PB11

#define MCP3221_I2C_CLOCK_SPEED 100000
#define MCP3221_GPIO_RCC        RCC_AHB1Periph_GPIOB
#define MCP3221_I2C_PORT        GPIOB

#define MCP3221_I2C             I2C2
#define MCP3221_AF              GPIO_AF_I2C2
#define MCP3221_RCC             RCC_APB1Periph_I2C2

#define MCP3221_SCL_SRC_PIN     GPIO_PinSource10
#define MCP3221_SDA_SRC_PIN     GPIO_PinSource11

#define MCP3221_I2C_SCL_PIN     GPIO_Pin_10
#define MCP3221_I2C_SDA_PIN     GPIO_Pin_11

#define MCP3221_READ_ADDRESS    0x9B   //¶ÁÊý¾ÝµØÖ·

void MCP3221_Init(void);

unsigned char MCP3221_GetState(void);

//12 bit AD
unsigned short MCP3221_ReadADC(unsigned char Channel);

#endif
