#ifndef __ADS1256_H_
#define	__ADS1256_H_

//***************************
//		Pin assign	   	
//		GPIOA12	--- CS
//		GPIOA11	---	RESET
//		GPIOA10 --- DRDY

//		GPIOB10	--- SCK
//		GPIOB14 --- DOUT (MISO)
//		GPIOB15 --- DIN  (MOSI)
//***************************					

//�˿ڶ���

//NOTICE: DO NOT USE PA14 AND PA13 !!!
//NOTICE: DO NOT USE PB3 (JTAG)

#define ADS1256_SPI         							SPI3
#define ADS1256_GPIO											GPIOC
#define ADS1256_RCC_GPIO	    						RCC_AHB1Periph_GPIOC
#define ADS1256_RCC_SPI 		    					RCC_APB1Periph_SPI3
#define ADS1256_GPIO_SPEED								GPIO_Speed_50MHz

//SCK:PC10
#define ADS1256_SPI_SCK_PORT							GPIOC
#define ADS1256_SPI_SCK_PIN			    			GPIO_Pin_10
#define ADS1256_SPI_SCK_CLK 							RCC_AHB1Periph_GPIOC
#define ADS1256_SPI_SCK_PINSOURCE 		    GPIO_PinSource10
#define ADS1256_SPI_SCK_AF 								GPIO_AF_SPI3

//MOSI:PC12(DIN)

#define ADS1256_SPI_MOSI_PORT 						GPIOC
#define ADS1256_SPI_MOSI_PIN 							GPIO_Pin_12
#define ADS1256_SPI_MOSI_CLK 							RCC_AHB1Periph_GPIOC
#define ADS1256_SPI_MOSI_PINSOURCE 				GPIO_PinSource12
#define ADS1256_SPI_MOSI_AF 							GPIO_AF_SPI3

//MISO:PC11(DOUT)
#define ADS1256_SPI_MISO_PORT 			  		GPIOC
#define ADS1256_SPI_MISO_PIN 							GPIO_Pin_11
#define ADS1256_SPI_MISO_CLK 							RCC_AHB1Periph_GPIOC
#define ADS1256_SPI_MISO_PINSOURCE 				GPIO_PinSource11
#define ADS1256_SPI_MISO_AF 							GPIO_AF_SPI3

//CS(NSS):PC6

#define ADS1256_CS_PORT 									GPIOC
#define ADS1256_CS_PIN 										GPIO_Pin_6
#define ADS1256_CS_RCC										RCC_AHB1Periph_GPIOC


//READY:PC7

#define ADS1256_DRDY_PORT                 GPIOC  
#define ADS1256_DRDY_PIN                  GPIO_Pin_7
#define ADS1256_DRDY_RCC                  RCC_AHB1Periph_GPIOC


//RESET:PC8
#define ADS1256_RESET_PORT                GPIOC   
#define ADS1256_RESET_PIN                 GPIO_Pin_8
#define ADS1256_RESET_RCC                 RCC_AHB1Periph_GPIOC




// define commands 
#define ADS1256_CMD_WAKEUP   0x00 
#define ADS1256_CMD_RDATA    0x01 
#define ADS1256_CMD_RDATAC   0x03 
#define ADS1256_CMD_SDATAC   0x0f 
#define ADS1256_CMD_RREG     0x10 
#define ADS1256_CMD_WREG     0x50 
#define ADS1256_CMD_SELFCAL  0xf0 
#define ADS1256_CMD_SELFOCAL 0xf1 
#define ADS1256_CMD_SELFGCAL 0xf2 
#define ADS1256_CMD_SYSOCAL  0xf3 
#define ADS1256_CMD_SYSGCAL  0xf4 
#define ADS1256_CMD_SYNC     0xfc 
#define ADS1256_CMD_STANDBY  0xfd 
#define ADS1256_CMD_REST    0xfe 
 
// define the ADS1256 register values 
#define ADS1256_STATUS       0x00   
#define ADS1256_MUX          0x01   
#define ADS1256_ADCON        0x02   
#define ADS1256_DRATE        0x03   
#define ADS1256_IO           0x04   
#define ADS1256_OFC0         0x05   
#define ADS1256_OFC1         0x06   
#define ADS1256_OFC2         0x07   
#define ADS1256_FSC0         0x08   
#define ADS1256_FSC1         0x09   
#define ADS1256_FSC2         0x0A 
 
 
// define multiplexer codes 
#define ADS1256_MUXP_AIN0   0x00 
#define ADS1256_MUXP_AIN1   0x10 
#define ADS1256_MUXP_AIN2   0x20 
#define ADS1256_MUXP_AIN3   0x30 
#define ADS1256_MUXP_AIN4   0x40 
#define ADS1256_MUXP_AIN5   0x50 
#define ADS1256_MUXP_AIN6   0x60 
#define ADS1256_MUXP_AIN7   0x70 
#define ADS1256_MUXP_AINCOM 0x80 
 
#define ADS1256_MUXN_AIN0   0x00 
#define ADS1256_MUXN_AIN1   0x01 
#define ADS1256_MUXN_AIN2   0x02 
#define ADS1256_MUXN_AIN3   0x03 
#define ADS1256_MUXN_AIN4   0x04 
#define ADS1256_MUXN_AIN5   0x05 
#define ADS1256_MUXN_AIN6   0x06 
#define ADS1256_MUXN_AIN7   0x07 
#define ADS1256_MUXN_AINCOM 0x08   
 
 
// define gain codes 
#define ADS1256_GAIN_1      0x00 
#define ADS1256_GAIN_2      0x01 
#define ADS1256_GAIN_4      0x02 
#define ADS1256_GAIN_8      0x03 
#define ADS1256_GAIN_16     0x04 
#define ADS1256_GAIN_32     0x05 
#define ADS1256_GAIN_64     0x06 
//#define ADS1256_GAIN_64     0x07 
 
//define drate codes 
#define ADS1256_DRATE_30000SPS   0xF0 
#define ADS1256_DRATE_15000SPS   0xE0 
#define ADS1256_DRATE_7500SPS   0xD0 
#define ADS1256_DRATE_3750SPS   0xC0 
#define ADS1256_DRATE_2000SPS   0xB0 
#define ADS1256_DRATE_1000SPS   0xA1 
#define ADS1256_DRATE_500SPS    0x92 
#define ADS1256_DRATE_100SPS    0x82 
#define ADS1256_DRATE_60SPS     0x72 
#define ADS1256_DRATE_50SPS     0x63 
#define ADS1256_DRATE_30SPS     0x53 
#define ADS1256_DRATE_25SPS     0x43 
#define ADS1256_DRATE_15SPS     0x33 
#define ADS1256_DRATE_10SPS     0x23 
#define ADS1256_DRATE_5SPS      0x13 
#define ADS1256_DRATE_2_5SPS    0x03

void ADS1256_Init(void);   //��ʼ��ADS1256

//24 bit AD
//NOTE: Still Testing
unsigned int ADS1256_ReadADC(unsigned char channel);

int ADS1256_ReadADC_Signed(unsigned char channel);

#endif



