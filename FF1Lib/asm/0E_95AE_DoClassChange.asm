define lut_promotion $9586
define dlgflg_reentermap $56
define ch_class0 $6100
define ch_class1 $6140
define ch_class2 $6180
define ch_class3 $61C0

DoClassChange:
	LDX ch_class0
	LDA lut_promotion, X
	STA ch_class0

	LDX ch_class1
	LDA lut_promotion, X
	STA ch_class1

	LDX ch_class2
	LDA lut_promotion, X
	STA ch_class2

	LDX ch_class3
	LDA lut_promotion, X
	STA ch_class3

	INC dlgflg_reentermap  ; set flag to redraw party
	RTS
