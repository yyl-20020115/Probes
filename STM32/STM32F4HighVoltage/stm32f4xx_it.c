/* Includes ------------------------------------------------------------------*/
#include "stm32f4xx_it.h"
#include "stm32f4xx_exti.h"
#include "stm32f4xx_usart.h"
#include "stm32f4xx_gpio.h"
#include "main.h"
#include "AMMeter.h"

#include <stdio.h>

uint8_t TxBuffer[256] = {0};  
uint8_t RxBuffer[256] = {0};

uint8_t AmBuffer[16] = {0};
__IO uint8_t AmCounter  = 0;

__IO uint8_t TxCounter = 0x00;
__IO uint8_t RxCounter = 0x00; 

uint8_t rx_flag = 0;
uint8_t tx_flag = 0;

extern void TimingDelay_Decrement(void);


/** @addtogroup STM32F4_Discovery_Peripheral_Examples
  * @{
  */

/** @addtogroup IO_Toggle
  * @{
  */ 

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/******************************************************************************/
/*            Cortex-M4 Processor Exceptions Handlers                         */
/******************************************************************************/

/**
  * @brief   This function handles NMI exception.
  * @param  None
  * @retval None
  */
void NMI_Handler(void)
{
}

/**
  * @brief  This function handles Hard Fault exception.
  * @param  None
  * @retval None
  */
void HardFault_Handler(void)
{
    /* Go to infinite loop when Hard Fault exception occurs */
    while (1)
    {
    }
}

/**
  * @brief  This function handles Memory Manage exception.
  * @param  None
  * @retval None
  */
void MemManage_Handler(void)
{
    /* Go to infinite loop when Memory Manage exception occurs */
    while (1)
    {
    }
}

/**
  * @brief  This function handles Bus Fault exception.
  * @param  None
  * @retval None
  */
void BusFault_Handler(void)
{
    /* Go to infinite loop when Bus Fault exception occurs */
    while (1)
    {
    }
}

/**
  * @brief  This function handles Usage Fault exception.
  * @param  None
  * @retval None
  */
void UsageFault_Handler(void)
{
    /* Go to infinite loop when Usage Fault exception occurs */
    while (1)
    {
    }
}

/**
  * @brief  This function handles SVCall exception.
  * @param  None
  * @retval None
  */
void SVC_Handler(void)
{
}

/**
  * @brief  This function handles Debug Monitor exception.
  * @param  None
  * @retval None
  */
void DebugMon_Handler(void)
{
}

/**
  * @brief  This function handles PendSVC exception.
  * @param  None
  * @retval None
  */
void PendSV_Handler(void)
{
}

/**
  * @brief  This function handles SysTick Handler.
  * @param  None
  * @retval None
  */
void SysTick_Handler(void)
{
    
    TimingDelay_Decrement();
}

/******************************************************************************/
/*                 STM32F4xx Peripherals Interrupt Handlers                   */
/*  Add here the Interrupt Handler for the used peripheral(s) (PPP), for the  */
/*  available peripheral interrupt handler's name please refer to the startup */
/*  file (startup_stm32f4xx.s).                                               */
/******************************************************************************/

/**
  * @brief  This function handles PPP interrupt request.
  * @param  None
  * @retval None
  */
/*void PPP_IRQHandler(void)
{
}*/
/**
  * @brief  This function handles External line 0 interrupt request.
  * @param  None
  * @retval None
  */


void EXTI0_IRQHandler(void)
{
    if(EXTI_GetITStatus(EXTI_Line0) != RESET)
    {
        /* Clear the EXTI line 0 pending bit */
        EXTI_ClearITPendingBit(EXTI_Line0);
    }
}

extern uint32_t DAValue;
uint32_t ReceivedCount = 0;
uint32_t ReceivedData = 0;
/**
  * @brief  This function handles USART1 global interrupt request.
  * @param  None
  * @retval : None
  */
void USART1_IRQHandler(void)     
{
    if(USART_GetITStatus(USART1, USART_IT_RXNE) != RESET)	   //�ж϶��Ĵ����Ƿ�ǿ�
    {	
        uint8_t v = (uint8_t)USART_ReceiveData(USART1);   //�����Ĵ��������ݻ��浽���ջ�������
				ReceivedCount ++;
			  ReceivedData |=v;
			  
				if(ReceivedCount == 2)
				{
					if ((ReceivedData & 0xFFFFF000)==0)
					{
						DAValue = ReceivedData;
					}
					else
					{
						//Bad input, redo						
					}
					ReceivedData = 0;
					ReceivedCount = 0;
				}
				ReceivedData <<=8;
    }
  
    if(USART_GetITStatus(USART1, USART_IT_TXE) != RESET)                   //�����Ϊ�˱���STM32 USART ��һ���ֽڷ�����ȥ��BUG 
    { 
        USART_ITConfig(USART1, USART_IT_TXE, DISABLE);					     //��ֹ�����������жϣ� 
    }	
}
void USART2_IRQHandler(void)     
{	
    if(USART_GetITStatus(USART2, USART_IT_RXNE) != RESET)	   //�ж϶��Ĵ����Ƿ�ǿ�
    {	
			uint16_t ch = USART_ReceiveData(USART2);
			
    }
  
    if(USART_GetITStatus(USART2, USART_IT_TXE) != RESET)                   //�����Ϊ�˱���STM32 USART ��һ���ֽڷ�����ȥ��BUG 
    { 
        USART_ITConfig(USART2, USART_IT_TXE, DISABLE);					     //��ֹ�����������жϣ� 
    }	
}


/**-------------------------------------------------------
  * @������ TIM1_IRQHandler
  * @����   TIM1�жϴ����������ٲ���ģʽ
  * @����   ��
  * @����ֵ ��
***------------------------------------------------------*/
void ROTATIONSPEED_IRQ_HANDLER(void)
{
}
/**
  * @}
  */ 

/**
  * @}
  */ 

/******************* (C) COPYRIGHT 2011 STMicroelectronics *****END OF FILE****/
