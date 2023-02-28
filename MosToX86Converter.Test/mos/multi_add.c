
volatile unsigned char a = 0;
volatile unsigned char b = 0;
volatile unsigned char c = 0;
volatile unsigned char d = 0;
volatile unsigned char e = 0;

void __attribute__ ((noinline)) inca()
{
    a++;
}

int main()
{
    begin:
    inca();
    inca();
    inca();
    inca();
    if (a != 0)
        goto begin;

    b++;
    b++;
    b++;
    b++;
    if (b != 0)
        goto begin;

    c++;
    c++;
    c++;
    c++;
    if (c != 0)
        goto begin;

    d++;
    d++;
    d++;
    d++;
    if (d != 0)
        goto begin;

    e += 0x40;
    if (e != 0)
        goto begin;
}
