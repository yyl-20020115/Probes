/* Includes ------------------------------------------------------------------*/

#include "main.h"
#include <stdio.h>
#include <string.h>

#define INVALID_DAVALUE 0xffff

__IO uint32_t TimingDelay;
__IO uint32_t TimingValue = 0;
__IO uint32_t DelayValue = DEFAULT_LOOP_DELAY;
__IO uint16_t DAValue = INVALID_DAVALUE;

/****************************************************************************
* ��		�ƣ�void Delay_us(__IO uint32_t nTime)
* ��		�ܣ���ʱ��ʱ���� 10usΪ��λ
* ��ڲ�������
* ���ڲ�������
* ˵		����
* ���÷������� 
****************************************************************************/	
void Delay_us(__IO uint32_t nTime)
{ 
		TimingDelay = nTime;

		while(TimingDelay != 0);
}
void Delay_ms(__IO uint32_t nTime)
{ 
		Delay_us(nTime*1000);
}

/****************************************************************************
* ��		�ƣ�void TimingDelay_Decrement(void)
* ��		�ܣ���ȡ���ĳ���
* ��ڲ�������
* ���ڲ�������
* ˵		����
* ���÷������� 
****************************************************************************/	
void TimingDelay_Decrement(void)
{
		TimingValue++;
		if (TimingDelay != 0x00)
		{ 
				TimingDelay--;
		}
}

/****************************************************************************/
void USART_Config(USART_TypeDef* USARTx){

		USART_InitTypeDef USART_InitStructure;
		
		USART_InitStructure.USART_BaudRate = 115200;						//����115200bps
		USART_InitStructure.USART_WordLength = USART_WordLength_8b;		//����λ8λ
		USART_InitStructure.USART_StopBits = USART_StopBits_1;			//ֹͣλ1λ
		USART_InitStructure.USART_Parity = USART_Parity_No;				//��У��λ
		USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;	 //��Ӳ������
		USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;					//�շ�ģʽ

		/* Configure USARTx */
		USART_Init(USARTx, &USART_InitStructure);							//���ô��ڲ�������


		/* Enable USARTx Receive and Transmit interrupts */
		USART_ITConfig(USARTx, USART_IT_RXNE, ENABLE);										//ʹ�ܽ����ж�
		USART_ITConfig(USARTx, USART_IT_TXE, ENABLE);						//ʹ�ܷ��ͻ�����ж�	 

		/* Enable the USARTx */
		USART_Cmd(USARTx, ENABLE);	
}

	

/*
*********************************************************************************************************
*	�� �� ��: fgetc
*	����˵��: �ض���getc��������������ʹ��scanf�����Ӵ���1��������
*	��		��: ��
*	�� �� ֵ: ��
*********************************************************************************************************
*/
int fgetc(FILE *f)
{
	/* �ȴ�����1�������� */
	while (USART_GetFlagStatus(MAIN_USART, USART_FLAG_RXNE) == RESET);

	return (int)USART_ReceiveData(MAIN_USART);
}


/*
*********************************************************************************************************
*	�� �� ��: fputc
*	����˵��: �ض���putc��������������ʹ��printf�����Ӵ���1��ӡ���
*	��		��: ��
*	�� �� ֵ: ��
*********************************************************************************************************
*/
int fputc(int ch, FILE *f)
{
	/* дһ���ֽڵ�USART1 */
	USART_SendData(MAIN_USART, (uint8_t) ch);

	/* �ȴ����ͽ��� */
	while (USART_GetFlagStatus(MAIN_USART, USART_FLAG_TC) == RESET)
	{}
	return ch;
}

void Default_Init(void)
{
	GPIO_InitTypeDef	 GPIO_InitStructure;
	NVIC_InitTypeDef	 NVIC_InitStructure;
		/*!< At this stage the microcontroller clock setting is already configured, 
			 this is done through SystemInit() function which is called from startup
			 file (startup_stm32f4xx.s) before to branch to application main.
			 To reconfigure the default setting of SystemInit() function, refer to
				system_stm32f4xx.c file
		 */
	//1us interval
	if (SysTick_Config(SystemCoreClock /N_1M))
		{ 
				/* Capture error */ 
				while (1);
		}

		RCC_AHB1PeriphClockCmd(
				RCC_AHB1Periph_GPIOA | 
				RCC_AHB1Periph_GPIOB |
				RCC_AHB1Periph_GPIOC |
				RCC_AHB1Periph_GPIOD |
				RCC_AHB1Periph_GPIOE |
				RCC_AHB1Periph_GPIOF |
				RCC_AHB1Periph_GPIOG,
				ENABLE);
	
		RCC_APB2PeriphClockCmd(MAIN_USART_RCC, ENABLE);
				
		/* Enable SYSCFG clock */
		
		//MAIN_USART_PORT
		GPIO_InitStructure.GPIO_Pin = MAIN_USART_PIN_TX | MAIN_USART_PIN_RX;
		GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
		GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
		GPIO_Init(MAIN_USART_PORT, &GPIO_InitStructure);		 

		GPIO_PinAFConfig(MAIN_USART_PORT, MAIN_USART_SRC_TX, MAIN_USART_AF);		
		GPIO_PinAFConfig(MAIN_USART_PORT, MAIN_USART_SRC_RX, MAIN_USART_AF);	

		//Interrupt for MAIN_USART
		NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1); 
		NVIC_InitStructure.NVIC_IRQChannel = MAIN_USART_IRQ;
		NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0; 
		NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0; 
		NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; 
		NVIC_Init(&NVIC_InitStructure);

		USART_Config(MAIN_USART);
				
}


#ifdef	USE_FULL_ASSERT

/**
	* @brief	Reports the name of the source file and the source line number
	*				 where the assert_param error has occurred.
	* @param	file: pointer to the source file name
	* @param	line: assert_param error line source number
	* @retval None
	*/
void assert_failed(uint8_t* file, uint32_t line)
{ 
		/* User can add his own implementation to report the file name and line number,
		 ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */

		/* Infinite loop */
		while (1)
		{
		}
}
#endif
void DAC_GPIO_Config(void)
{
	GPIO_InitTypeDef	 GPIO_InitStructure;
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA,ENABLE);
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AN;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_4 | GPIO_Pin_5;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
	GPIO_Init(GPIOA,&GPIO_InitStructure);
}

void DAC_WriteValue(unsigned short value){
	DAC_SetChannel1Data(DAC_Align_12b_R,value);

	DAC_SoftwareTriggerCmd(DAC_Channel_1,ENABLE);
}

void DAC_Config(void)
{
	DAC_InitTypeDef DAC_InitStructure;
	
	DAC_GPIO_Config();
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_DAC,ENABLE);
	DAC_InitStructure.DAC_WaveGeneration = DAC_WaveGeneration_None;
	DAC_InitStructure.DAC_Trigger = DAC_Trigger_Software;
	DAC_InitStructure.DAC_OutputBuffer = DAC_OutputBuffer_Enable;
	DAC_Init(DAC_Channel_1,&DAC_InitStructure);

	DAC_Cmd(DAC_Channel_1,ENABLE);
	DAC_WriteValue(0);
}

//#define DO_CALI
/**
	* @brief	Main program
	* @param	None
	* @retval None
*/
int main(void)
{
		int Data = 0;
	  int Middle = 0;
		Default_Init();
		DAC_Config();
	
	
		HX711_Init();
		HX711_Calibrate();
#ifdef DO_CALI
		HX711_Calibrate();
	printf("CALI-0:%d",RealZeroLevel);
		HX711_Calibrate();
	printf("CALI-1:%d",RealZeroLevel);	
		HX711_Calibrate();
	printf("CALI-2:%d",RealZeroLevel);	
#endif	
	
		while (TRUE)
		{						
		  if(DAValue!=INVALID_DAVALUE)
			{
				DAC_WriteValue((unsigned short)(DAValue&0x03ff));
				DAValue = INVALID_DAVALUE;
			}
			
			Data = HX711_Read();
			Middle = HX711_GetMiddleFilterValue(Data);
			
		  printf("WEIGHT:%08X,%08X,%08X,%08X,%08X\n",Data,Middle,Real500gLevel,Real100gLevel,RealZeroLevel);//12+40=52

			Delay_us(DelayValue);

	  }
}

/**
	* @}
	*/ 

/**
	* @}
	*/ 

/******************* (C) COPYRIGHT 2011 STMicroelectronics *****END OF FILE****/
