#ifndef __Compass_H
#define __Compass_H

#if defined AK8975_SECONDARY
#define SUPPORTS_AK89xx_HIGH_SENS   (0x00)
#define AK89xx_FSR                  (9830)
#elif defined AK8963_SECONDARY
#define SUPPORTS_AK89xx_HIGH_SENS   (0x10)
#define AK89xx_FSR                  (4915)
#endif

#ifdef AK89xx_SECONDARY
#define AKM_REG_WHOAMI      (0x00)

#define AKM_REG_ST1         (0x02)
#define AKM_REG_HXL         (0x03)
#define AKM_REG_ST2         (0x09)

#define AKM_REG_CNTL        (0x0A)
#define AKM_REG_ASTC        (0x0C)
#define AKM_REG_ASAX        (0x10)
#define AKM_REG_ASAY        (0x11)
#define AKM_REG_ASAZ        (0x12)

#define AKM_DATA_READY      (0x01)
#define AKM_DATA_OVERRUN    (0x02)
#define AKM_OVERFLOW        (0x80)
#define AKM_DATA_ERROR      (0x40)
/*AK8975ÄÚ²¿µØÖ·***************************************************/
#define USET_CTR            (0x6A)
#define INT_PIN_CFG			(0x37)
#define RAW_COMPASS			(0X49)
//#define COMPASS_ADDR	    (0X18)
#define COMPASS_ADDR	    (0X0C)
#define BIT_AUX_IF_EN       (0x20)
#define BIT_ANY_RD_CLR      (0x10)
/***************************************************************/

#define AKM_BIT_SELF_TEST   (0x40)
#define AKM_POWER_DOWN          (0x00 | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_SINGLE_MEASUREMENT  (0x01 | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_FUSE_ROM_ACCESS     (0x0F | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_MODE_SELF_TEST      (0x08 | SUPPORTS_AK89xx_HIGH_SENS)

#define AKM_WHOAMI      (0x48)
#endif

//void mpu_compass_Init(void);
 //void mpu_get_compass_reg(void);
void ANBT_MPU9150_I2CBYPASS_CFG_FUN(void);
void ANBT_MPU6050_I2CBYPASS_CFG_FUN(void);
u8 ANBT_AK8975_MAG_WHOAMI_FUN(void);
 int mpu_set_bypass(unsigned char bypass_on);
 int mpu_get_compass_reg(void);

#endif