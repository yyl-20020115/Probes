#include "main.h"
//JTAG PINS: PB3,PB4,PA13,PA14,PA15
//TOUCH-SPI1:PA15 ->Conflict with JTAG

__IO u32 LocalTime=0; 

int RealZeroLevel = 0;
int Real100gLevel = 0;
int Real500gLevel = 0;

int _ZeroLevel = 8482827;
int _100gLevel = 8558457;
int _500gLevel = 8860429;

int  EnableDisp = 1;
int  ClearDisp = 0;

float Weight = 0.0f;
#define		N	12
//PIN:PC6,PC7
#define HX_RCC RCC_AHB1Periph_GPIOC
#define HX_PORT GPIOC
#define ADSK_PIN GPIO_Pin_6
#define ADDO_PIN GPIO_Pin_7

int DataStore[N] ={0};

//HX711 AD读数
int ReadCount(void)
{
    int Count=0;
    int i=0;

    GPIO_ResetBits(HX_PORT,ADSK_PIN);
	
    Count = 0;
    while( GPIO_ReadInputDataBit(HX_PORT,ADDO_PIN));
    for (i = 0; i < 24; i++)
    {
				GPIO_SetBits(HX_PORT,ADSK_PIN);
        Count = Count << 1;
				GPIO_ResetBits(HX_PORT,ADSK_PIN);
        if(GPIO_ReadInputDataBit(HX_PORT,ADDO_PIN)) 
				{
					Count |=1;
				}
    }
		GPIO_SetBits(HX_PORT,ADSK_PIN);
    Count = Count ^ 0x800000;

		GPIO_ResetBits(HX_PORT,ADSK_PIN);

    return Count;
}


//滤波
int GetMiddleFilterValue(int rc)
{
	int max = 0;
	int min = 0;
	int64_t sum = 0;
	int i = 0;

	max = min = sum = DataStore[0] = rc; 	

	for(i=N-1; i!= 0; i--)
	{
		if(DataStore[i] > max) 
			max = DataStore[i];
		else if(DataStore[i] < min) 
			min = DataStore[i];

		sum = sum + DataStore[i];
		DataStore[i] = DataStore[i - 1];
	}
	
	i = N - 2;
	sum = sum - max - min;// + i/2;
	sum = (sum/i);

	return (int)sum;
}
void Scale_Init()
{
	GPIO_InitTypeDef GPIO_InitStructure;//初始化结构体

	RCC_AHB1PeriphClockCmd(HX_RCC, ENABLE);//GPIO外设时钟使能
	//PB7=ADDO
	GPIO_InitStructure.GPIO_Pin = ADDO_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;//状态为输入
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;//无上下拉
	GPIO_Init(HX_PORT, &GPIO_InitStructure);//初始化
	//PB6=ADSK
	GPIO_InitStructure.GPIO_Pin =  ADSK_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//状态为输入
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;//无上下拉
	GPIO_Init(HX_PORT, &GPIO_InitStructure);//初始化
}

void Calibrate(int32_t rc)
{	
	int Delta = 0;
	int i = 0;
	for(i = 0;i<N;i++) //fill buffer
	{
		GetMiddleFilterValue(ReadCount());
	}
	
	RealZeroLevel = GetMiddleFilterValue(ReadCount());
	
	Delta = RealZeroLevel - _ZeroLevel;
  Real100gLevel =	_100gLevel + Delta;
	Real500gLevel = _500gLevel + Delta;
		
}
int main(void)
{
  int k = 0;
	u8 buffer[256] = {0};   
	int data = 0;
	int value = 0;
	int middle = 0;
  Key_Init();//按键初始化
	LED_Init();//LED灯初始化
	Scale_Init();
	LCD_Init();//LCD显示屏初始化
	
	Calibrate(ReadCount());
	
	COM1Init(115200);//串口1初始化

	LCD_String(64,10,(const u8*)"STM32 Scale",BLUE);

	while(1)
	{
		
		k = KEY_Scan();

		value = ReadCount();
		data = middle = GetMiddleFilterValue(value);
		printf("WEIGHT:%08X,%08X,%08X,%08X,%08X\n",value,middle,Real500gLevel,Real100gLevel,RealZeroLevel);//12+40=52
		switch(k){
			case 1:
				_ZeroLevel = middle;
			  snprintf((char*)buffer,sizeof(buffer)-1,"000g = %8d        ",RealZeroLevel);
			  LCD_String(24,36,buffer,RED);
				break;
			case 2: //100g value
				Real100gLevel =  middle;
			  snprintf((char*)buffer,sizeof(buffer)-1,"100g = %8d        ",Real100gLevel);
			  LCD_String(24,54,buffer,RED);
				break;
			case 3: //500g value
				Real500gLevel =  middle;
			  snprintf((char*)buffer,sizeof(buffer)-1,"500g = %8d        ",Real500gLevel);
			  LCD_String(24,72,buffer,RED);
				break;
			case 4: //cali
		  	Calibrate(value);
			  snprintf((char*)buffer,sizeof(buffer)-1,"000g = %8d        ",RealZeroLevel);
			  LCD_String(24,36,buffer,RED);
				break;
		  default: //default show
				if(EnableDisp)
				{
					snprintf((char*)buffer,sizeof(buffer)-1,"data = %8d        ",data);
					LCD_String(24,90,buffer,BLUE);
					Weight = (Real500gLevel!=Real100gLevel) ? (float)(data - RealZeroLevel)/(float)(Real500gLevel - Real100gLevel) * 400.0f : 0.0f;
					snprintf((char*)buffer,sizeof(buffer)-1,"weight = %4.4lfg        ",Weight);
					LCD_String(24,108,buffer,BLUE);
				}
				else
			  {
					if(ClearDisp)
					{
						memset(buffer,' ',sizeof(buffer)-1);
						buffer[sizeof(buffer)-1] = '\0';
						LCD_String(24,90,buffer,BLUE);
						LCD_String(24,108,buffer,BLUE);
						ClearDisp = 0;
					}
				}
				break;				
		}
		
	}
}


