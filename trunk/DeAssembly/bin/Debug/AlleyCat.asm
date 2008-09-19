#sample of the N64's V4300i mips processor assembly (Alley Cat 64 rom).
lb      $s7, 1240($at)          #0x00000000 = 80 37 12 40
sync                            #0x00000004
lb      $zero, 0000($at)        #0x00000008
sllv    $v0, $zero, 11          #0x0000000C
andi    $t3, $v1, 3375          #0x00000010
???                             #0x00000014 = 05 F4 E6 98 wtf?
nop                             #0x00000018
nop                             #0x0000001C
cop0                            #0x00000020
???                             #0x00000024 = 79 63 61 74 wtf?
ori     $s4, $s1, 2020          #0x00000028
addi    $zero, $at, 2020        #0x0000002C
addi    $zero, $at, 2020        #0x00000030
nop                             #0x00000034
???                             #0x00000038 = 00 00 00 4E wtf?
beql    $k0, $t5, 00011440      #0x0000003C
mtc0    $zero, $t5              #0x00000040
mtc0    $zero, $t1              #0x00000044
mtc0    $zero, $t3              #0x00000048
lui     $t0, A470               #0x0000004C
addiu   $t0, $t0,0000           #0x00000050
lw      $t1, 000C($t0)          #0x00000054
bne     $t1, $zero, 00000410    #0x00000058
nop
addiu

#steps: 1) replace labels 2) comment out ???