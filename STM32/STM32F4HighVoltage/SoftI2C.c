#include "SoftI2C.h"

void SOFTI2C_SCL_0(SOFTI2C* s)		{  GPIO_ResetBits(s->SCL_Port,s->SCL_Pin);}
void SOFTI2C_SCL_1(SOFTI2C* s) 		{  GPIO_SetBits(s->SCL_Port,s->SCL_Pin);}
void SOFTI2C_SDA_0(SOFTI2C* s) 		{  GPIO_ResetBits(s->SDA_Port,s->SDA_Pin);}
void SOFTI2C_SDA_1(SOFTI2C* s) 		{  GPIO_SetBits(s->SDA_Port,s->SDA_Pin);}

unsigned char   SOFTI2C_SDA_STATE(SOFTI2C* s)        { return GPIO_ReadInputDataBit(s->SDA_Port,s->SDA_Pin) != 0; }

extern void Delay_us(unsigned int nTime);

/*******************************************************************************
* ��������:SOFTI2C_Delay                                                                     
* ��    ��:��ʱ����                                                                     
*                                                                               
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:                                                                     
* �޸�����:2010��6��8��                                                                    
*******************************************************************************/
void SOFTI2C_NOP(SOFTI2C* s)
{
    unsigned int i = 20, j = 0;
    unsigned int sum = 0;

    if(s->DelayValue>0)
    {
        Delay_us(s->DelayValue);
    }
    else
    {

        while(i--)
        {
            for (j = 0; j < 10; j++)
            {
                sum += i; 
            }
        }
        sum = i;
    }
}

/*******************************************************************************
* ��������:SOFTI2C_Initialize                                                                     
* ��    ��:I2C��ʼ������                                                                     
*                                                                               
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:                                                                     
* �޸�����:2010��6��8��                                                                    
*******************************************************************************/
void SOFTI2C_Init(SOFTI2C* s)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode=GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;

    GPIO_InitStructure.GPIO_Pin = s->SCL_Pin; 
    GPIO_Init(s->SCL_Port, &GPIO_InitStructure);

    //SDA: OD Mode, BOTH IN AND OUT
    GPIO_InitStructure.GPIO_Mode=GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
    GPIO_InitStructure.GPIO_Pin = s->SDA_Pin; 
    GPIO_Init(s->SDA_Port, &GPIO_InitStructure);


    SOFTI2C_SDA_1(s);
    SOFTI2C_SCL_0(s); 
}

/*******************************************************************************
* ��������:SOFTI2C_START                                                                     
* ��    ��:��������                                                                     
*                                                                               
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:��                                                                     
* ��    ��:                                                                     
* �޸�����:2010��6��8��                                                                    
*******************************************************************************/
unsigned char SOFTI2C_START(SOFTI2C* s)
{ 
    SOFTI2C_SDA_1(s); 
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_1(s); 
    SOFTI2C_NOP(s);    

    if(!SOFTI2C_SDA_STATE(s))
    {
        return s->State = SOFTI2C_BUS_BUSY;
    }
    
    SOFTI2C_SDA_0(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_0(s);  
    SOFTI2C_NOP(s); 

    if(SOFTI2C_SDA_STATE(s))
    {
        return s->State = SOFTI2C_BUS_ERROR;
    } 

    return SOFTI2C_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_START_SHT 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char SOFTI2C_START_SHT(SOFTI2C* s)
{
    SOFTI2C_SDA_1(s);
    SOFTI2C_SCL_0(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SDA_1(s); 
    SOFTI2C_SCL_1(s); 
    SOFTI2C_NOP(s);

    if(!SOFTI2C_SDA_STATE(s))
    {
        return s->State = SOFTI2C_BUS_BUSY;
    }
    
    SOFTI2C_SDA_0(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_0(s);  
    SOFTI2C_NOP(s); 

    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SDA_1(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_0(s);
    SOFTI2C_NOP(s);

    return s->State = SOFTI2C_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_STOP 
*/
/* --------------------------------------------------------------------------*/
void SOFTI2C_STOP(SOFTI2C* s)
{
    SOFTI2C_SDA_0(s); 
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_1(s); 
    SOFTI2C_NOP(s);    

    SOFTI2C_SDA_1(s);
    SOFTI2C_NOP(s);
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_SendACK 
*/
/* --------------------------------------------------------------------------*/
void SOFTI2C_SendACK(SOFTI2C* s)
{
    SOFTI2C_SDA_0(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_0(s); 
    SOFTI2C_NOP(s); 
    SOFTI2C_SDA_1(s);
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_SendNACK 
*/
/* --------------------------------------------------------------------------*/
void SOFTI2C_SendNACK(SOFTI2C* s)
{
    SOFTI2C_SDA_1(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_0(s); 
    SOFTI2C_NOP(s);
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_SendByte 
* 
* @Param: Data
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char SOFTI2C_SendByte(SOFTI2C* s,unsigned char Data)
{
    unsigned char i = 0;
    
    SOFTI2C_SCL_0(s);
    for(i=0;i<8;i++)
    {  
        //---------���ݽ���----------
        if(Data&0x80)
        {
            SOFTI2C_SDA_1(s);
        }
        else
        {
            SOFTI2C_SDA_0(s);
        } 
        Data<<=1;
        SOFTI2C_NOP(s);
        //---���ݽ�������һ����ʱ----

        //----����һ��������[������] 
        SOFTI2C_SCL_1(s);
        SOFTI2C_NOP(s);
        SOFTI2C_SCL_0(s);
        SOFTI2C_NOP(s);//��ʱ,��ֹSCL��û��ɵ�ʱ�ı�SDA,�Ӷ�����START/STOP�ź�
        //---------------------------   
    }
    
    //���մӻ���Ӧ�� 
    SOFTI2C_SDA_1(s); 
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);   
    
    if(SOFTI2C_SDA_STATE(s))
    {
        SOFTI2C_SCL_0(s);
        SOFTI2C_SDA_1(s);

        return s->State = SOFTI2C_NACK;
    }
    else
    {
        SOFTI2C_SCL_0(s);
        SOFTI2C_SDA_1(s);
        
        return s->State = SOFTI2C_ACK;  
    }    
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_ReceiveByte 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char SOFTI2C_ReceiveByte(SOFTI2C* s)
{
    unsigned char i = 0,Dat = 0;
    SOFTI2C_SDA_1(s);
    SOFTI2C_SCL_0(s); 

    for(i=0;i<8;i++)
    {
        SOFTI2C_SCL_1(s);//����ʱ��������[������],�ôӻ�׼�������� 
        SOFTI2C_NOP(s); 
        Dat<<=1;
        if(SOFTI2C_SDA_STATE(s)) //������״̬
        {
            Dat|=0x01; 
        }   
        SOFTI2C_SCL_0(s);//׼�����ٴν�������  
        SOFTI2C_NOP(s);//�ȴ�����׼����         
    }
    return Dat;
}
