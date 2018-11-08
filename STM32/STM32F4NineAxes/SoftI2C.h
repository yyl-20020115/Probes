#ifndef __SOFTI2_H__
#define __SOFTI2_H__

#include "stm32f4xx_gpio.h"

#define true  1
#define false 0 
#define bool uint8_t


#define SOFTI2C_I2C_PORT    GPIOB

#define SOFTI2C_PIN_SCL 	GPIO_Pin_10
#define SOFTI2C_PIN_SDA 	GPIO_Pin_11

#define SOFTI2C_ACK   	    0
#define SOFTI2C_READY 	    0
#define SOFTI2C_NACK 	    1
#define SOFTI2C_BUS_BUSY  	2
#define SOFTI2C_BUS_ERROR 	3

#define SOFTI2C_RETRY_COUNT 3
#define SOFTI2C_MAX_WAITACK 65535


typedef struct
{
    int ID;
    
    GPIO_TypeDef* SCL_Port;
    
    GPIO_TypeDef* SDA_Port;
    
    unsigned int SCL_Pin;
    
    unsigned int SDA_Pin;
    
    unsigned int DelayValue;
    
    unsigned char State;
    
} SOFTI2C;




void SOFTI2C_Init(SOFTI2C* s);
unsigned char SOFTI2C_START(SOFTI2C* s);
unsigned char SOFTI2C_START_SHT(SOFTI2C* s);
void SOFTI2C_STOP(SOFTI2C* s);

unsigned char SOFTI2C_SendByte(SOFTI2C* s,unsigned char Data);
unsigned char SOFTI2C_ReceiveByte(SOFTI2C* s);

void SOFTI2C_SendACK(SOFTI2C* s);
void SOFTI2C_SendNACK(SOFTI2C* s);
bool SOFTI2C_WaitACK(SOFTI2C* s);

#endif
