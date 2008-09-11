Sll $a2 $a2, 2
Sll $a3, $a3, 2
add $v0, $zero, $zero
add $t0, $zero, $zero
outer:			#Outer:	add $t4, $a0, $t0
add $t4, $a0, $t0
lw $t4, 0($t4)
add $t1, $zero, $zero
inner:			#Inner: add $t3, $a1, $t1
add $t3, $a1, $t1
lw $t3, 0($t3)
bne $t3, $t4, skip
addi $v0, $v0, 1
skip:			#Skip:	addi $t1, $t1, 4
addi $t1, $t1, 4
bne $t1, $a3, inner
addi $t0, $t0, 4
bne $t0, $a2, outer
