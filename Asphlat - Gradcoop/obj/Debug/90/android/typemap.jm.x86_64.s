	/* Data SHA1: 88c30e53ba1fbe0d046f223729c7a2e9e2760b29 */
	.file	"typemap.jm.inc"

	/* Mapping header */
	.section	.data.jm_typemap,"aw",@progbits
	.type	jm_typemap_header, @object
	.p2align	2
	.global	jm_typemap_header
jm_typemap_header:
	/* version */
	.long	1
	/* entry-count */
	.long	121
	/* entry-length */
	.long	216
	/* value-offset */
	.long	89
	.size	jm_typemap_header, 16

	/* Mapping data */
	.type	jm_typemap, @object
	.global	jm_typemap
jm_typemap:
	.size	jm_typemap, 26137
	.include	"typemap.jm.inc"