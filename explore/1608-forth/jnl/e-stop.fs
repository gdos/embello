\ stop mode experiment
\ needs core.fs
cr cr reset
cr

     RCC $48 + constant RCC-APB1SMENR
     RCC $50 + constant RCC-CSR

$40007C00 constant LPTIM1
   LPTIM1 $00 + constant LPTIM-ISR
   LPTIM1 $04 + constant LPTIM-ICR
   LPTIM1 $08 + constant LPTIM-IER
   LPTIM1 $0C + constant LPTIM-CFGR
   LPTIM1 $10 + constant LPTIM-CR
   LPTIM1 $14 + constant LPTIM-CMP
   LPTIM1 $18 + constant LPTIM-ARR
   LPTIM1 $1C + constant LPTIM-CNT

$40007000 constant PWR
      PWR $0 + constant PWR-CR
      PWR $4 + constant PWR-CSR

: lptim? ( -- )
  LPTIM1
  cr  ." ISR " dup @ h.2 space 4 +
      ." ICR " dup @ h.2 space 4 +
      ." IER " dup @ h.2 space 4 +
     ." CFGR " dup @ hex.      4 +
       ." CR " dup @ h.2 space 4 +
      ." CMP " dup @ h.4 space 4 +
      ." ARR " dup @ h.4 space 4 +
      ." CNT " dup @ h.4 space drop ;

: +lptim
  0 bit RCC-CSR bis!              \ set LSION
  begin 1 bit RCC-CSR bit@ until  \ wait for LSIRDY
  %01 18 lshift RCC-CCIPR bis!    \ use LSI clock
  31 bit RCC-APB1ENR bis!         \ enable LPTIM1
  31 bit RCC-APB1SMENR bis!       \ also enable in sleep mode
  %111 9 lshift LPTIM-CFGR !      \ 128 prescaler
  0 bit LPTIM-CR bis!             \ set ENABLE
  37000 128 / LPTIM-ARR !         \ 1s timout
;

: hsi-off 0 bit RCC-CR bic! ;

: lp-stop
  65KHz hsi-off  \ fake for now, i.e. still running, but very slowly
;

: stop1s ( -- )
  1 bit LPTIM-CR bis!               \ set SNGSTRT
  lp-stop
  begin 1 bit LPTIM-ISR bit@ until  \ wait for ARRM
  1 bit LPTIM-ICR bis!              \ clear ARRM
;

: lp-blink
  begin
    3 0 do stop1s loop
    led iox!
  key? until ;

rf69-init rf-sleep led-off 2.1MHz
1000 systick-hz  \ FIXME if omitted, blink startup stalls for several seconds
+lptim 

( clock-hz ) clock-hz @ .
( PWR-CR ) PWR-CR @ hex.
( PWR-CSR ) PWR-CSR @ hex.
( RCC-CR ) RCC-CR @ hex.
( RCC-CSR ) RCC-CSR @ hex.
( RCC-CCIPR ) RCC-CCIPR @ hex.

lptim?

1234 ms lp-blink