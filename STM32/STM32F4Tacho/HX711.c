#include "HX711.h"
#include "stm32f4_discovery.h"

void Delay_ms(__IO uint32_t nTime);
void Delay_us(__IO uint32_t nTime);


#define HX711_DELAY()   Delay_us(1)

void HX711_Init(void)
{
    GPIO_InitTypeDef   GPIO_InitStructure;
    
    RCC_AHB1PeriphClockCmd(HX711_RCC,ENABLE);

    GPIO_InitStructure.GPIO_Pin =  HX711_SCK_PIN;				 //PD0 = SCK, OUT MODE
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
    GPIO_Init(HX711_PORT, &GPIO_InitStructure);

    GPIO_InitStructure.GPIO_Pin =  HX711_DOUT_PIN;				 //PD1 = DOUT, IN-OUT MODE
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;
    GPIO_Init(HX711_PORT, &GPIO_InitStructure);
}

int HX711_Read(void)
{
    int Value =0;
    char i = 0;

    //SCK =0;
    GPIO_ResetBits(HX711_PORT, HX711_SCK_PIN);
    
    HX711_DELAY();
    //时间会很长
    while(GPIO_ReadInputDataBit(HX711_PORT,HX711_DOUT_PIN));
    
    HX711_DELAY();
  
    for(i = 0;i<24;i++)
    {
        GPIO_SetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 1
        Value <<=1;
				HX711_DELAY(); 
        GPIO_ResetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 0
        if(GPIO_ReadInputDataBit(HX711_PORT,HX711_DOUT_PIN))
        {
            Value|=1;
        }
				HX711_DELAY(); 
    }
    //sck 25 A 128
    GPIO_SetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 1

		

		Value = Value ^ 0x800000;

		
    HX711_DELAY(); 
 
    GPIO_ResetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 0
    HX711_DELAY(); 

    
    return Value;
}

