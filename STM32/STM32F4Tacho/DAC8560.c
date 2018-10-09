#include "stm32f4_discovery.h"
#include "DAC8560.h"


#define DAC8560_SCLK_1_()   GPIO_SetBits(DAC8560_GPIO_PORT_CLK, DAC8560_GPIO_PIN_CLK)
#define DAC8560_SCLK_0_()   GPIO_ResetBits(DAC8560_GPIO_PORT_CLK, DAC8560_GPIO_PIN_CLK)
#define DAC8560_SYNC_1_()   GPIO_SetBits(DAC8560_GPIO_PORT_SYNC, DAC8560_GPIO_PIN_SYNC)
#define DAC8560_SYNC_0_()   GPIO_ResetBits(DAC8560_GPIO_PORT_SYNC, DAC8560_GPIO_PIN_SYNC)
#define DAC8560_DIN__1_()   GPIO_SetBits(DAC8560_GPIO_PORT_DIN, DAC8560_GPIO_PIN_DIN)
#define DAC8560_DIN__0_()   GPIO_ResetBits(DAC8560_GPIO_PORT_DIN, DAC8560_GPIO_PIN_DIN)
#define DAC8560_DELAY()     Delay_us(1)    

void DAC8560_Init(void)
{
    GPIO_InitTypeDef   GPIO_InitStructure;
    
    DAC8560_SCLK_1_();
    
    /* GPIO Peripheral clock enable */
    RCC_AHB1PeriphClockCmd(DAC8560_RCC, ENABLE);


    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;

    GPIO_InitStructure.GPIO_Pin =  DAC8560_GPIO_PIN_SYNC;
    GPIO_Init(DAC8560_GPIO_PORT_SYNC, &GPIO_InitStructure);
    
    GPIO_InitStructure.GPIO_Pin =  DAC8560_GPIO_PIN_CLK;
    GPIO_Init(DAC8560_GPIO_PORT_CLK, &GPIO_InitStructure);
    
    GPIO_InitStructure.GPIO_Pin =  DAC8560_GPIO_PIN_DIN;
    GPIO_Init(DAC8560_GPIO_PORT_DIN, &GPIO_InitStructure);
    
}

//16 BIT DA
void DAC8560_WriteDAC(unsigned short DAValue,unsigned char op)
{
	  int i = 0;

    int Command = ((((unsigned int)op)<<16)|(unsigned int)DAValue)<<8;
    
    
    DAC8560_SYNC_0_();
    DAC8560_DELAY();
    
    
    for(i=0;i<24;i++)
    {
        if(Command &0x80000000)
        {
            DAC8560_DIN__1_();
        }
        else
        {
            DAC8560_DIN__0_();
        }
        
        DAC8560_DELAY();
        
        DAC8560_SCLK_1_();
        DAC8560_DELAY();
        Command<<=1;
        DAC8560_SCLK_0_(); 
        DAC8560_DELAY();

    }
 
    DAC8560_SYNC_1_();   
    DAC8560_DELAY();
    DAC8560_SCLK_0_(); 
    DAC8560_DELAY();
}

