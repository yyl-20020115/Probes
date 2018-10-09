#include "stm32f4_discovery.h"
#include "DAC121S101.h"

#define DAC121S101_SCLK_1_()   GPIO_SetBits(DAC121S101_GPIO_PORT_CLK, DAC121S101_GPIO_PIN_CLK)
#define DAC121S101_SCLK_0_()   GPIO_ResetBits(DAC121S101_GPIO_PORT_CLK, DAC121S101_GPIO_PIN_CLK)
#define DAC121S101_SYNC_1_()   GPIO_SetBits(DAC121S101_GPIO_PORT_SYNC, DAC121S101_GPIO_PIN_SYNC)
#define DAC121S101_SYNC_0_()   GPIO_ResetBits(DAC121S101_GPIO_PORT_SYNC, DAC121S101_GPIO_PIN_SYNC)
#define DAC121S101_DIN__1_()   GPIO_SetBits(DAC121S101_GPIO_PORT_DIN, DAC121S101_GPIO_PIN_DIN)
#define DAC121S101_DIN__0_()   GPIO_ResetBits(DAC121S101_GPIO_PORT_DIN, DAC121S101_GPIO_PIN_DIN)
#define DAC121S101_DELAY()     Delay_us(1)    

void DAC121S101_Init(void)
{
    GPIO_InitTypeDef   GPIO_InitStructure;
 
    /* GPIO Peripheral clock enable */
    RCC_AHB1PeriphClockCmd(DAC121S101_GPIO_RCC, ENABLE);


    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;

    GPIO_InitStructure.GPIO_Pin =  DAC121S101_GPIO_PIN_SYNC;
    GPIO_Init(DAC121S101_GPIO_PORT_SYNC, &GPIO_InitStructure);
    
    GPIO_InitStructure.GPIO_Pin =  DAC121S101_GPIO_PIN_CLK;
    GPIO_Init(DAC121S101_GPIO_PORT_CLK, &GPIO_InitStructure);
    
    GPIO_InitStructure.GPIO_Pin =  DAC121S101_GPIO_PIN_DIN;
    GPIO_Init(DAC121S101_GPIO_PORT_DIN, &GPIO_InitStructure);
    
}

//12 BIT DA
void DAC121S101_WriteDAC(unsigned short DAValue,unsigned char op)
{
    int i = 0;
    unsigned short Command = (op & 0x03);

    Command <<=12;
    
    Command |= (DAValue & 0x0fff);

    
    DAC121S101_SYNC_0_();
    DAC121S101_DELAY();    
    
    for(i=0;i<16;i++)
    {
        DAC121S101_SCLK_1_();
        DAC121S101_DELAY();
        if(Command & 0x8000)
        {
            DAC121S101_DIN__1_();
        }
        else
        {
            DAC121S101_DIN__0_();
        }
        Command<<=1;
        DAC121S101_SCLK_0_(); 
        DAC121S101_DELAY();
    }
 
    DAC121S101_SYNC_1_();   
    DAC121S101_DELAY();
    DAC121S101_SCLK_0_(); 
    DAC121S101_DELAY();
    
    DAC121S101_DIN__0_();
    DAC121S101_DELAY();
    
}

