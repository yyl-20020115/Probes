#include "stm32f4_discovery.h"
#include "stm32f4xx_tim.h"
#include "stm32f4xx_gpio.h"
#include "FrequencyMeter.h"
#include <string.h>

__IO uint32_t Period = 0;
__IO uint32_t LastCapture = 0;
__IO uint32_t ThisCapture = 0;
__IO uint32_t CaptureNumber  = 0;
__IO uint32_t PulsePeriod = 0;
__IO uint32_t PeriodBuf[PERIOD_BUFSIZE] = {0};
__IO uint32_t PeriodIndex = 0;


TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure = {0};

void FM_Init(void)
{
	GPIO_InitTypeDef FMGPIO_InitStructure = {0};
	NVIC_InitTypeDef FMNVIC_InitStructure = {0};
	TIM_ICInitTypeDef TIM_ICInitStructure = {0};

	
	RCC_APB1PeriphClockCmd(FREQUENCYMETER_TIM_RCC, ENABLE);
	RCC_AHB1PeriphClockCmd(FREQUENCYMETER_TIM_PORT_RCC, ENABLE);

	
	FMGPIO_InitStructure.GPIO_Pin =  FREQUENCYMETER_PIN;
	FMGPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	FMGPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	FMGPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
	FMGPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; //STM32F4:50MHz-200MHz
	GPIO_Init(FREQUENCYMETER_PORT, &FMGPIO_InitStructure);
	
	GPIO_PinAFConfig(FREQUENCYMETER_PORT, FREQUENCYMETER_PIN_SOURCE, FREQUENCYMETER_TIM_AF); 

	

	TIM_TimeBaseStructure.TIM_Prescaler = (SystemCoreClock / FREQUENCYMETER_SYS_FREQUENCY) -1;

	TIM_TimeBaseStructure.TIM_Period = FREQUENCYMETER_PERIOD - 1;	
	TIM_TimeBaseStructure.TIM_ClockDivision = 0;
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseStructure.TIM_RepetitionCounter = 0;
	TIM_TimeBaseInit(FREQUENCYMETER_TIM, &TIM_TimeBaseStructure);

   
	TIM_ICInitStructure.TIM_Channel = FREQUENCYMETER_CHANNEL;
	TIM_ICInitStructure.TIM_ICPolarity = TIM_ICPolarity_Falling;
	TIM_ICInitStructure.TIM_ICSelection = TIM_ICSelection_DirectTI;
	
	//NOTICE: here should be TIM_ICPSC_DIV1,
	//but it get double speed of the real value,
	//so we have to divide it by 2.
	TIM_ICInitStructure.TIM_ICPrescaler = TIM_ICPSC_DIV1;
	TIM_ICInitStructure.TIM_ICFilter = FREQUENCYMETER_FILTER;

	TIM_ICInit(FREQUENCYMETER_TIM, &TIM_ICInitStructure);

	FMNVIC_InitStructure.NVIC_IRQChannel = FREQUENCYMETER_TIM_IRQ;
	FMNVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	FMNVIC_InitStructure.NVIC_IRQChannelSubPriority = 1;
	FMNVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&FMNVIC_InitStructure);

	TIM_ITConfig(FREQUENCYMETER_TIM, FREQUENCYMETER_TIM_IT_CC, ENABLE);	

	TIM_Cmd(FREQUENCYMETER_TIM, ENABLE);
	
}
void FM_ClearPeriod(void)
{
    Period = 0;  
}

uint32_t FM_GetPeriod(void)
{
	return Period;
}
uint32_t FM_GetSysFrequency(void)
{
	return FREQUENCYMETER_SYS_FREQUENCY;
}

float FM_GetFrequency(void)
{
		if(Period!=0)
		{
			return FREQUENCYMETER_SYS_FREQUENCY /(float)(Period);
		}
		else
		{
			return 0.0f;
		}
}

unsigned int FM_UpdatePeriod(void)
{
	unsigned int index = 0;
	unsigned char max_index=0,min_index=0;
	unsigned short ValueTemp=0;
	unsigned int ValueAvg=0;//平均值
	unsigned int index_total=0;

	//取得最大值及其编号
	ValueTemp = PeriodBuf[0];
	for(index = 1;index < PERIOD_BUFSIZE; index++)
	{
		if(ValueTemp < PeriodBuf[index])
		{
			max_index = index;
			ValueTemp = PeriodBuf[index];
		}
	}
	//取得最小值及其编号
	ValueTemp = PeriodBuf[0];
	for(index = 1;index < PERIOD_BUFSIZE; index++)
	{
		if(ValueTemp > PeriodBuf[index])
		{
			min_index = index;
			ValueTemp = PeriodBuf[index];
		}
	}
	
	//去掉最大值和最小值 求和
	for(index = 0;index < PERIOD_BUFSIZE;index++)
	{
		if((index != min_index) && (index != max_index))
		{
			ValueAvg += PeriodBuf[index];
			index_total++;
		}
	}
	
	ValueAvg = ValueAvg/index_total;//取平均值
	
	return ValueAvg;
}


void FM_IRQ(void)
{

    if(TIM_GetITStatus(FREQUENCYMETER_TIM, FREQUENCYMETER_TIM_IT_CC) != RESET) 
    {			
			  TIM_ClearITPendingBit(FREQUENCYMETER_TIM, FREQUENCYMETER_TIM_IT_CC);
			
					if(CaptureNumber == 0)
					{
							LastCapture = FREQUENCYMETER_CAPTURE();
							CaptureNumber=1;
					}
					else if(CaptureNumber == 1)
					{
							ThisCapture = FREQUENCYMETER_CAPTURE();
		
							if(TIM_GetFlagStatus(TIM1,TIM_FLAG_CC1OF) == SET)
							{
								TIM_ClearFlag(FREQUENCYMETER_TIM,TIM_FLAG_CC1OF);
								PulsePeriod = ((FREQUENCYMETER_PERIOD - LastCapture) + ThisCapture); 								
							}
							else
							{
								PulsePeriod = (ThisCapture - LastCapture); 
							}


							PeriodBuf[PeriodIndex++] = PulsePeriod;

							if(PeriodIndex == PERIOD_BUFSIZE)
							{
									Period = FM_UpdatePeriod();

									memset((void*)PeriodBuf,0,sizeof(uint32_t)*PERIOD_BUFSIZE);

									PeriodIndex = 0;
							}
							
							CaptureNumber = 0;
					}

	
    }
}
