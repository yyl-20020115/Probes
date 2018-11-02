#include "usart.h"
#include <stdio.h>
//PIN:PA2(TX),PA3(RX)
//��ʼ������1
void COM2Init(u32 BaudRate)
{
  	GPIO_InitTypeDef GPIO_InitStructure;
  	USART_InitTypeDef USART_InitStructure;
  	NVIC_InitTypeDef	 NVIC_InitStructure;

  	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA, ENABLE);//����ʱ��ʹ�� 
  	RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART2, ENABLE);
  	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2 | GPIO_Pin_3;
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
  	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
  	GPIO_Init(GPIOA, &GPIO_InitStructure);//��ʼ������1��GPIO   
	
  	GPIO_PinAFConfig(GPIOA, GPIO_PinSource2, GPIO_AF_USART2);//���Ӹ�������  
  	GPIO_PinAFConfig(GPIOA, GPIO_PinSource3, GPIO_AF_USART2);
		NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1); 
		NVIC_InitStructure.NVIC_IRQChannel = USART2_IRQn;
		NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0; 
		NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0; 
		NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; 
		NVIC_Init(&NVIC_InitStructure);

	
  	USART_InitStructure.USART_BaudRate = BaudRate;//����������
  	USART_InitStructure.USART_WordLength = USART_WordLength_8b;//8λ����ģʽ
  	USART_InitStructure.USART_StopBits = USART_StopBits_1;//1λֹͣλ
  	USART_InitStructure.USART_Parity = USART_Parity_No;//����żУ��λ
  	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//��Ӳ���������
  	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;//˫��ģʽ 
  	USART_Init(USART2, &USART_InitStructure);
  	USART_Cmd(USART2, ENABLE);
  	USART_ClearFlag(USART2, USART_FLAG_TC);//�崫����ɱ�־
}

//����print֧��
#if 1
#pragma import(__use_no_semihosting)                              
struct __FILE 
{ 
	int handle; 
}; 
FILE __stdout;         
int _sys_exit(int x) 
{ 
	return (x = x); 
} 
int fputc(int ch, FILE *f)
{
  	USART_SendData(USART2, (u8) ch);//����1����һ���ַ�
  	while (USART_GetFlagStatus(USART2, USART_FLAG_TC) == RESET);//�ȴ��������
  	return ch;
}
#endif
