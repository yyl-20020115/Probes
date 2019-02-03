//#include "stm32f4_discovery.h"
#include "ADS1256.h"
#include "main.h"

#define ADS1256_DELAY() delay_us(5)

//-----------------------------------------------------------------//
//	��		�ܣ�	ģ��SPIͨ��
//	��ڲ���: /	���͵�SPI����
//	���ڲ���: /	���յ�SPI����
//	ȫ�ֱ���: /
//	��		ע: 	���ͽ��պ���
//-----------------------------------------------------------------//
unsigned char ADS1256_SPI_WriteByte(unsigned char TxData)
{
	unsigned char RxData=0;

	while(SPI_I2S_GetFlagStatus(ADS1256_SPI,SPI_I2S_FLAG_TXE)==RESET); //																									 
	SPI_I2S_SendData(ADS1256_SPI,TxData);

	while(SPI_I2S_GetFlagStatus(ADS1256_SPI,SPI_I2S_FLAG_RXNE)==RESET);

	RxData=SPI_I2S_ReceiveData(ADS1256_SPI);

	return RxData;
} 
//-----------------------------------------------------------------//
//	��		�ܣ�ADS1256 д����
//	��ڲ���: /
//	���ڲ���: /
//	ȫ�ֱ���: /
//	��		ע: ��ADS1256�е�ַΪregaddr�ļĴ���д��һ���ֽ�databyte
//-----------------------------------------------------------------//
void ADS1256WREG(unsigned char regaddr,unsigned char databyte)
{
	GPIO_ResetBits(ADS1256_CS_PORT, ADS1256_CS_PIN);
	
	while(GPIO_ReadInputDataBit(ADS1256_DRDY_PORT,ADS1256_DRDY_PIN))
	{
		//empty
	}//��ADS1256_DRDYΪ��ʱ����д�Ĵ���
	
	//��Ĵ���д�����ݵ�ַ
	ADS1256_SPI_WriteByte(ADS1256_CMD_WREG | (regaddr & 0x0F));
		
	//д�����ݵĸ���n-1
	ADS1256_SPI_WriteByte(0x00);
	//��regaddr��ַָ��ļĴ���д������databyte
	ADS1256_SPI_WriteByte(databyte);
	GPIO_SetBits(ADS1256_CS_PORT, ADS1256_CS_PIN);
}


void ADS1256_SPI_Init(void)
{
	SPI_InitTypeDef	SPI_InitStructure;
	GPIO_InitTypeDef GPIO_InitStructure;
	/****Initial ADS1256_SPI******************/
	RCC_AHB1PeriphClockCmd(ADS1256_RESET_RCC |ADS1256_DRDY_RCC , ENABLE);
	
	
	//RESET
	GPIO_InitStructure.GPIO_Pin = ADS1256_RESET_PIN; 
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 
	GPIO_InitStructure.GPIO_Speed = ADS1256_GPIO_SPEED;
	
	GPIO_Init(ADS1256_RESET_PORT, &GPIO_InitStructure);	
	GPIO_ResetBits(ADS1256_RESET_PORT, ADS1256_RESET_PIN );


	//DRDY
	GPIO_InitStructure.GPIO_Pin = ADS1256_DRDY_PIN; 
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP; //SHOULD PULL UP
	GPIO_Init(ADS1256_DRDY_PORT, &GPIO_InitStructure);
	
	
	//SPI
	
	RCC_AHB1PeriphClockCmd(ADS1256_RCC_GPIO , ENABLE);
		
	/* Enable ADS1256_GPIO_CLK and ADS1256_GPIO_CLK */
	RCC_APB1PeriphClockCmd(ADS1256_RCC_SPI, ENABLE);

	
		
	/* Configure ADS1256_SPI pins: SCK, MISO and MOSI */
	GPIO_InitStructure.GPIO_Pin =	ADS1256_SPI_SCK_PIN | ADS1256_SPI_MISO_PIN | ADS1256_SPI_MOSI_PIN;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_Speed = ADS1256_GPIO_SPEED;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	GPIO_Init(ADS1256_GPIO, &GPIO_InitStructure);
	
	
		//ADS1256_SPI NSS (CS)
	GPIO_InitStructure.GPIO_Pin = ADS1256_CS_PIN;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_Speed = ADS1256_GPIO_SPEED;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL; 
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_Init(ADS1256_CS_PORT, &GPIO_InitStructure);
	
	GPIO_SetBits(ADS1256_CS_PORT, ADS1256_CS_PIN);

	
	
	GPIO_PinAFConfig(ADS1256_SPI_SCK_PORT,ADS1256_SPI_SCK_PINSOURCE,ADS1256_SPI_SCK_AF);
	GPIO_PinAFConfig(ADS1256_SPI_MOSI_PORT,ADS1256_SPI_MOSI_PINSOURCE,ADS1256_SPI_MOSI_AF);
	GPIO_PinAFConfig(ADS1256_SPI_MISO_PORT,ADS1256_SPI_MISO_PINSOURCE,ADS1256_SPI_MISO_AF);

 
	/* ADS1256_SPI configuration */ 
	SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex; //ADS1256_SPI����Ϊ����ȫ˫��
	SPI_InitStructure.SPI_Mode = SPI_Mode_Master;										//����ADS1256_SPIΪ��ģʽ
	SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;									//SPI���ͽ���8λ֡�ṹ
	SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;									 //����ʱ���ڲ�����ʱ��ʱ��Ϊ�͵�ƽ
	SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;								 //��һ��ʱ���ؿ�ʼ��������
	SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;									//NSS�ź��������ʹ��SSIλ������
	SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_256; //���岨����Ԥ��Ƶ��ֵ:������Ԥ��ƵֵΪ8
	SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;			 //���ݴ����MSBλ��ʼ
	SPI_InitStructure.SPI_CRCPolynomial = 7;				 //CRCֵ����Ķ���ʽ
	SPI_Init(ADS1256_SPI, &SPI_InitStructure);
	/* Enable SPI	*/
	SPI_Cmd(ADS1256_SPI, ENABLE);	
	
			
	//DO RESET
	GPIO_SetBits(ADS1256_RESET_PORT, ADS1256_RESET_PIN); 
		
	delay_us(0x100);
}	



//��ʼ��ADS1256
void ADS1256_Init(void)
{
	ADS1256_SPI_Init();

	
	ADS1256WREG(ADS1256_STATUS,0x06);							 // ��λ��ǰ��У׼��ʹ�û���
	ADS1256WREG(ADS1256_ADCON,0x00);								// �Ŵ���1
	ADS1256WREG(ADS1256_DRATE,ADS1256_DRATE_30000SPS);	// ����30000SPS

	ADS1256WREG(ADS1256_IO,0x00);							 
}




//��ȡADֵ
unsigned int ADS1256ReadData(void)	
{
	unsigned int value=0;
	GPIO_ResetBits(ADS1256_CS_PORT, ADS1256_CS_PIN);

	while(GPIO_ReadInputDataBit(ADS1256_DRDY_PORT,ADS1256_DRDY_PIN))
	{		
		//��ADS1256_DRDYΪ��ʱ����д�Ĵ��� 
	}
	//	ADS1256WREG(ADS1256_MUX,channel);		//����ͨ��
	ADS1256_SPI_WriteByte(ADS1256_CMD_SYNC);
	ADS1256_DELAY();
	ADS1256_SPI_WriteByte(ADS1256_CMD_WAKEUP);								 
	ADS1256_DELAY();
	ADS1256_SPI_WriteByte(ADS1256_CMD_RDATA);
	ADS1256_DELAY();
	
	value |= (((unsigned int)ADS1256_SPI_WriteByte(0xff)) << 16);
	ADS1256_DELAY();
	
	value |= (((unsigned int)ADS1256_SPI_WriteByte(0xff)) << 8);
	ADS1256_DELAY();
	value |= ((unsigned int)ADS1256_SPI_WriteByte(0xff));
	ADS1256_DELAY();
  
  value &=0x00ffffff;
	
	GPIO_SetBits(ADS1256_CS_PORT, ADS1256_CS_PIN); 
  
	return value;
}

//24 BIT AD
//-----------------------------------------------------------------//
//	��		�ܣ���ȡADS1256��·����
//	��ڲ���: /
//	���ڲ���: /
//	ȫ�ֱ���: /
//	��		ע: /
//-----------------------------------------------------------------//
unsigned int ADS1256_ReadADC(unsigned char channel)
{
		if(channel<8)
		{

				ADS1256WREG(ADS1256_MUX, (channel<<4) | ADS1256_MUXN_AINCOM);		//����ͨ��
				
								
				return ADS1256ReadData();//��ȡADֵ������24λ���ݡ�				
		}
		return 0;
}

int ADS1256_ReadADC_Signed(unsigned char channel)
{
	int result = 0;
	
	unsigned int UnsignedADCValue = ADS1256_ReadADC(channel);
	
	if( (UnsignedADCValue & 0x800000) ==  0x800000)
	{
		result = 0xff000000 | (UnsignedADCValue & 0xffffff);
	}
	else
	{
		result = UnsignedADCValue;
	}
	
	return result;
}
