\ Helpers to work with character buffers

\ stack>buffer copies i bytes from the stack into a buffer
: stack>buffer ( b1 b2 ... bi i c-addr -- c-addr len )
  2dup swap 2>r \ save c-addr len
  over + ( b1 b2 ... bi i c-end-addr )
  swap 0 ?do 1- tuck c! loop
  drop 2r>
  ;

: buffer. ( c-addr len -- ) \ print buffer like @<adr> <len> [ <c1> <c2> ... <cLen> ]
  ." @" over . dup . ." [ " 255 and 0 ?do dup i + c@ . loop drop ." ]" ;
