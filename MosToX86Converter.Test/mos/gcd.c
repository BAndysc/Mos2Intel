

#ifdef __CC65__
#include <6502.h>
unsigned short n1 = 5328;
unsigned short n2 = 1052;

unsigned short a = 0;
unsigned short b = 0;

unsigned short max = 0x01FF;

int main()
{
    *(unsigned char*)0x4567 = 2;
    while (a < max)
    {
        a++;
        b = 0;
        while (b < max)
        {
            b++;
            n1 = a;
            n2 = b;
            while(n1!=n2)
            {
                if(n1 > n2)
                    n1 -= n2;
                else
                    n2 -= n1;
            }
            // primitive debugging
            //*(unsigned char*)0xFFF7 = 2;
            //*(unsigned int*)0xFFF8 = n1;
            //BRK();
            //BRK();
            //*(unsigned char*)0xFFF7 = 0;
        }
    }
    *(unsigned char*)0xFFF7 = 1;
    BRK();
    BRK();
    return n1;
}
#else

volatile unsigned char n1[2] = {0xd0, 0x14};
volatile unsigned char n2[2]= {0x1c, 0x4};

volatile unsigned char a[2] = {0x0, 0x0};
volatile unsigned char b[2]= {0x0, 0x0};
volatile unsigned char zero[2]= {0x0, 0x0};
volatile unsigned char max[2]= {0xff, 0x3};

#define EQ(a, b) (a[0] == b[0] && a[1] == b[1])
#define GT(a, b) (a[1] > b[1] || (a[1] == b[1] && a[0] > b[0]))
#define LT(a, b) (!EQ(a, b) && !GT(a, b))

void inc(volatile unsigned char* a)
{
    a[0]++;
    if (a[0] == 0)
        a[1]++;
}

void sub(volatile unsigned char* a, volatile unsigned char* b)
{
    a[1] -= b[1];
    if (a[0] >= b[0])
        a[0] -= b[0];
    else
    {
        a[1]--;
        a[0] -= -255 + b[0] - 1;
    }
}

void set(volatile unsigned char* a, volatile unsigned char* b)
{
    a[0] = b[0];
    a[1] = b[1];
}

unsigned short toShort(unsigned char* a)
{
    return (unsigned short)a[0] | (unsigned short)a[1] << 8;
}

//#include <stdio.h>
int main()
{
    while (LT(a, max))
    {
        inc(a);
        set(b, zero);
        while (LT(b, max))
        {
            inc(b);
            set(n1, a);
            set(n2, b);
            
            //printf("a=%d, b=%d a==b? %d.   a>b? %d\n", toShort(n1), toShort(n2), EQ(n1, n2), GT(n1, n2));
            while (!EQ(n1, n2))
            {
                if (GT(n1, n2))
                    sub(n1, n2);
                else
                    sub(n2, n1);
            }
            //printf("GCD = %d", toShort(n1));
        }
    }
}

#endif
