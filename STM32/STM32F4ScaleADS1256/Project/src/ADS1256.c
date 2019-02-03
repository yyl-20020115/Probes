//#include "stm32f4_discovery.h"
#include "ADS1256.h"
#include "main.h"

#define ADS1256_DELAY() delay_us(5)

//-----------------------------------------------------------------//
//	功		能：	模拟SPI通信
//	入口参数: /	发送的SPI数据
//	出口参数: /	接收的SPI数据
//	全局变量: /
//	备		注: 	发送接收函数
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
//	功		能：ADS1256 写数据
//	入口参数: /
//	出口参数: /
//	全局变量: /
//	备		注: 向ADS1256中地址为regaddr的寄存器写入一个字节databyte
//-----------------------------------------------------------------//
void ADS1256WREG(unsigned char regaddr,unsigned char databyte)
{
	GPIO_ResetBits(ADS1256_CS_PORT, ADS1256_CS_PIN);
	
	while(GPIO_ReadInputDataBit(ADS1256_DRDY_PORT,ADS1256_DRDY_PIN))
	{
		//empty
	}//当ADS1256_DRDY为低时才能写寄存器
	
	//向寄存器写入数据地址
	ADS1256_SPI_WriteByte(ADS1256_CMD_WREG | (regaddr & 0x0F));
		
	//写入数据的个数n-1
	ADS1256_SPI_WriteByte(0x00);
	//向regaddr地址指向的寄存器写入数据databyte
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
	SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex; //ADS1256_SPI设置为两线全双工
	SPI_InitStructure.SPI_Mode = SPI_Mode_Master;										//设置ADS1256_SPI为主模式
	SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;									//SPI发送接收8位帧结构
	SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;									 //串行时钟在不操作时，时钟为低电平
	SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;								 //第一个时钟沿开始采样数据
	SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;									//NSS信号由软件（使用SSI位）管理
	SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_256; //定义波特率预分频的值:波特率预分频值为8
	SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;			 //数据传输从MSB位开始
	SPI_InitStructure.SPI_CRCPolynomial = 7;				 //CRC值计算的多项式
	SPI_Init(ADS1256_SPI, &SPI_InitStructure);
	/* Enable SPI	*/
	SPI_Cmd(ADS1256_SPI, ENABLE);	
	
			
	//DO RESET
	GPIO_SetBits(ADS1256_RESET_PORT, ADS1256_RESET_PIN); 
		
	delay_us(0x100);
}	



//初始化ADS1256
void ADS1256_Init(void)
{
	ADS1256_SPI_Init();

	
	ADS1256WREG(ADS1256_STATUS,0x06);							 // 高位在前、校准、使用缓冲
	ADS1256WREG(ADS1256_ADCON,0x00);								// 放大倍数1
	ADS1256WREG(ADS1256_DRATE,ADS1256_DRATE_30000SPS);	// 数据30000SPS

	ADS1256WREG(ADS1256_IO,0x00);							 
}




//读取AD值
unsigned int ADS1256ReadData(void)	
{
	unsigned int value=0;
	GPIO_ResetBits(ADS1256_CS_PORT, ADS1256_CS_PIN);

	while(GPIO_ReadInputDataBit(ADS1256_DRDY_PORT,ADS1256_DRDY_PIN))
	{		
		//当ADS1256_DRDY为低时才能写寄存器 
	}
	//	ADS1256WREG(ADS1256_MUX,channel);		//设置通道
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
//	功		能：读取ADS1256单路数据
//	入口参数: /
//	出口参数: /
//	全局变量: /
//	备		注: /
//-----------------------------------------------------------------//
unsigned int ADS1256_ReadADC(unsigned char channel)
{
		if(channel<8)
		{

				ADS1256WREG(ADS1256_MUX, (channel<<4) | ADS1256_MUXN_AINCOM);		//设置通道
				
								
				return ADS1256ReadData();//读取AD值，返回24位数据。				
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
