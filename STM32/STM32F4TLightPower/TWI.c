#include "stm32f4xx.h"
#include "stm32f4xx_gpio.h"
#include "TWI.h"


extern void Delay_ms(unsigned int n);


void TWI_SCL_0(void)		{  GPIO_ResetBits(TWI_I2C_PORT,TWI_PIN_SCL);}
void TWI_SCL_1(void) 		{  GPIO_SetBits(TWI_I2C_PORT,TWI_PIN_SCL);}
void TWI_SDA_0(void) 		{  GPIO_ResetBits(TWI_I2C_PORT,TWI_PIN_SDA);}
void TWI_SDA_1(void) 		{  GPIO_SetBits(TWI_I2C_PORT,TWI_PIN_SDA);}


unsigned char   TWI_SDA_STATE(void)        { return (TWI_I2C_PORT->IDR & TWI_PIN_SDA) != 0; }


/*******************************************************************************
* ��������:TWI_Delay                                                                     
* ��    ��:��ʱ����                                                                     
*                                                                               
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:                                                                     
* �޸�����:2010��6��8��                                                                    
*******************************************************************************/
void TWI_NOP(void)
{
    unsigned int i = 20, j = 0;
    unsigned int sum = 0;

    while(i--)
    {
        for (j = 0; j < 10; j++)
        {
            sum += i; 
        }
    }
    sum = i;
}

/*******************************************************************************
* ��������:TWI_Initialize                                                                     
* ��    ��:I2C��ʼ������                                                                     
*                                                                               
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:                                                                     
* �޸�����:2010��6��8��                                                                    
*******************************************************************************/
void TWI_Initialize(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode=GPIO_Mode_OUT;

    GPIO_InitStructure.GPIO_Pin = TWI_PIN_SDA | TWI_PIN_SCL; 
    GPIO_Init(TWI_I2C_PORT, &GPIO_InitStructure);


    TWI_SDA_1();
    TWI_SCL_0(); 
}

/*******************************************************************************
* ��������:TWI_START                                                                     
* ��    ��:��������                                                                     
*                                                                               
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:                                                                     
* �޸�����:2010��6��8��                                                                    
*******************************************************************************/
unsigned char TWI_START(void)
{ 
    TWI_SDA_1(); 
    TWI_NOP();

    TWI_SCL_1(); 
    TWI_NOP();    

    if(!TWI_SDA_STATE())
    {
        return TWI_BUS_BUSY;
    }
    
    TWI_SDA_0();
    TWI_NOP();

    TWI_SCL_0();  
    TWI_NOP(); 

    if(TWI_SDA_STATE())
    {
        return TWI_BUS_ERROR;
    } 

    return TWI_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_START_SHT 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char TWI_START_SHT(void)
{
    TWI_SDA_1();
    TWI_SCL_0();
    TWI_NOP();

    TWI_SDA_1(); 
    TWI_SCL_1(); 
    TWI_NOP();

    if(!TWI_SDA_STATE())
    {
        return TWI_BUS_BUSY;
    }
    
    TWI_SDA_0();
    TWI_NOP();

    TWI_SCL_0();  
    TWI_NOP(); 

    TWI_SCL_1();
    TWI_NOP();

    TWI_SDA_1();
    TWI_NOP();

    TWI_SCL_0();
    TWI_NOP();

    return TWI_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_STOP 
*/
/* --------------------------------------------------------------------------*/
void TWI_STOP(void)
{
    TWI_SDA_0(); 
    TWI_NOP();

    TWI_SCL_1(); 
    TWI_NOP();    

    TWI_SDA_1();
    TWI_NOP();
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_SendACK 
*/
/* --------------------------------------------------------------------------*/
void TWI_SendACK(void)
{
    TWI_SDA_0();
    TWI_NOP();
    TWI_SCL_1();
    TWI_NOP();
    TWI_SCL_0(); 
    TWI_NOP(); 
    TWI_SDA_1();
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_SendNACK 
*/
/* --------------------------------------------------------------------------*/
void TWI_SendNACK(void)
{
    TWI_SDA_1();
    TWI_NOP();
    TWI_SCL_1();
    TWI_NOP();
    TWI_SCL_0(); 
    TWI_NOP();
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_SendByte 
* 
* @Param: Data
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char TWI_SendByte(unsigned char Data)
{
    unsigned char i = 0;
    
    TWI_SCL_0();
    for(i=0;i<8;i++)
    {  
        //---------���ݽ���----------
        if(Data&0x80)
        {
            TWI_SDA_1();
        }
        else
        {
            TWI_SDA_0();
        } 
        Data<<=1;
        TWI_NOP();
        //---���ݽ�������һ����ʱ----

        //----����һ��������[������] 
        TWI_SCL_1();
        TWI_NOP();
        TWI_SCL_0();
        TWI_NOP();//��ʱ,��ֹSCL��û��ɵ�ʱ�ı�SDA,�Ӷ�����START/STOP�ź�
        //---------------------------   
    }
    
    //���մӻ���Ӧ�� 
    TWI_SDA_1(); 
    TWI_NOP();
    TWI_SCL_1();
    TWI_NOP();   
    
    if(TWI_SDA_STATE())
    {
        TWI_SCL_0();
        TWI_SDA_1();

        return TWI_NACK;
    }
    else
    {
        TWI_SCL_0();
        TWI_SDA_1();
        
        return TWI_ACK;  
    }    
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_ReceiveByte 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char TWI_ReceiveByte(void)
{
    unsigned char i = 0,Dat = 0;
    TWI_SDA_1();
    TWI_SCL_0(); 

    for(i=0;i<8;i++)
    {
        TWI_SCL_1();//����ʱ��������[������],�ôӻ�׼�������� 
        TWI_NOP(); 
        Dat<<=1;
        if(TWI_SDA_STATE()) //������״̬
        {
            Dat|=0x01; 
        }   
        TWI_SCL_0();//׼�����ٴν�������  
        TWI_NOP();//�ȴ�����׼����         
    }
    return Dat;
}
