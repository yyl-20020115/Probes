#ifndef __MAIN_H__
#define __MAIN_H__

#include "stm32f4_discovery.h"

#ifndef TRUE
#define TRUE    1
#endif

#ifndef FALSE
#define FALSE   0
#endif


//NOTICE: DO NOT USE PA14 AND PA13 !!!
//NOTICE: DO NOT USE PB3 (JTAG)



//PIN ASSIGNMENT:
//AD8591: 				PE0,PE1,PE2,PE3
//ADS1256:				PC6,PC7,PC8,PC10,PC11,PC12
//DAC121S101: 		PD13,PD14,PD15
//DAC8560:				PB0,PB1,PB2
//MPU6050:				PA8,PC9
//HX711:					PD0,PD1
//MCP3221:				PB10,PB11
//PCF8591:				PA8,PC9 (DUP WITH MPU6050)
//RotationSpeed: 	PA0
//TLC5615:				PF11,PF12,PF13 (Currently Used)
//USART1:					PB6(TX-RED),PB7(RX-ORANGE)
//FR:							PA3
//BR:							PA2
//USART2:         PA2(TX), PA3(RX), PD5(TX),PD6(RX) : AM-METER
//USART3:					PB10(TX), PB11(RX)
//USART6:         PC6(TX), PC7(RX)
//USART3,USART4:  PC10(TX), PC11(RX)
//USART6:					PG14(TX),PG9(RX)

//SWITCHES:				PF7,PF8
//00:							RotationMagnet	(RM)
//01:							VoltageControl	(VC)
//02:							VoltageMeter		(VM)


//Others:

//PB6:  I2C1_SCL  
//PB7:  I2C1_SDA
//PB10: I2C2_SCL  
//PB11: I2C2_SDA

#include "I2C.h"

//Number: 1M
#define N_1M	1000000
#define DEFAULT_LOOP_DELAY	10*1000

//USE USART1
//PB6:TX
//PB7:RX
#define MAIN_USART						USART1
#define MAIN_USART_PORT				GPIOB
#define MAIN_USART_PIN_TX			GPIO_Pin_6
#define MAIN_USART_PIN_RX			GPIO_Pin_7
#define MAIN_USART_RCC				RCC_APB2Periph_USART1
#define MAIN_USART_IRQ				USART1_IRQn
#define MAIN_USART_AF					GPIO_AF_USART1
#define MAIN_USART_SRC_TX			GPIO_PinSource6
#define MAIN_USART_SRC_RX			GPIO_PinSource7

//PD5:TX
//PD6:RX
#define AM_USART						  USART2
#define AM_USART_PORT				  GPIOD
#define AM_USART_PIN_TX			  GPIO_Pin_5
#define AM_USART_PIN_RX			  GPIO_Pin_6
#define AM_USART_RCC				  RCC_APB1Periph_USART2
#define AM_USART_IRQ				  USART2_IRQn
#define AM_USART_AF					  GPIO_AF_USART2
#define AM_USART_SRC_TX			  GPIO_PinSource5
#define AM_USART_SRC_RX			  GPIO_PinSource6



//SWITCHES
//PF7:BIT0
//PF8:BIT1

#define SWITCHES_PORT					GPIOF
#define	SWITCHES_PIN_0				GPIO_Pin_7
#define	SWITCHES_PIN_1				GPIO_Pin_8



//03:							RotationMagnet	(RM) (Default)
//02:							VoltageControl	(VC) (PF7=0)
//01:							VoltageMeter		(VM) (PF8=0)

#define SWITCHES_NO						0x00
#define SWITCHES_RM						0x03
#define SWITCHES_VC						0x02
#define SWITCHES_VM						0x01

#define SWITCHES_NO_STR				"NO"
#define SWITCHES_RM_STR				"RM"
#define SWITCHES_VC_STR				"VC"
#define SWITCHES_VM_STR				"VM"



//FR(Foward/Reverse)
#define FR_GPIO_PORT    			GPIOA
#define FR_GPIO_PIN     			GPIO_Pin_3
//BR(Break)
#define BR_GPIO_PORT					GPIOA
#define BR_GPIO_PIN						GPIO_Pin_2

//No device
#define DEVICE_NONE         	0

//24 bit AD Scale
#define DEVICE_SCALE_HX711  	1

//8 bit AD
#define DEVICE_ADC_PCF8591  	2

//12 bit AD
#define DEVICE_ADC_MCP3221		3

//24 bit AD
#define DEVICE_ADC_ADS1256  	4

//8 bit DA
#define DEVICE_DAC_PCF8591  	8

//10 bit DA
#define DEVICE_DAC_TLC5615  	9

//12 bit DA
#define DEVICE_DAC_DAC121S101	10

//16 bit DA
#define DEVICE_DAC_DAC8560  	11

//24 bit,70MHz DDS
#define DEVICE_DDS_AD9851			12

//Built-in Rotation Speed measurement
#define DEVICE_ROT_SPEED			13

#define DEVICE_MPU6050				14


void Delay_us(__IO uint32_t nTime);
void Delay_ms(__IO uint32_t nTime);

extern __IO unsigned int AMValue;

#endif
