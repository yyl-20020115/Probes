#ifndef __TWI_H__
#define __TWI_H__


#define TWI_I2C_PORT    GPIOB

#define TWI_PIN_SCL 	GPIO_Pin_6
#define TWI_PIN_SDA 	GPIO_Pin_7

#define TWI_ACK   	    0
#define TWI_READY 	    0
#define TWI_NACK 	    1
#define TWI_BUS_BUSY  	2
#define TWI_BUS_ERROR 	3

#define TWI_RETRY_COUNT 3


void TWI_Initialize(void);
unsigned char TWI_START(void);
unsigned char TWI_START_SHT(void);
void TWI_STOP(void);
unsigned char TWI_SendByte(unsigned char Data);
unsigned char TWI_ReceiveByte(void);
void TWI_SendACK(void);
void TWI_SendNACK(void);


#endif

