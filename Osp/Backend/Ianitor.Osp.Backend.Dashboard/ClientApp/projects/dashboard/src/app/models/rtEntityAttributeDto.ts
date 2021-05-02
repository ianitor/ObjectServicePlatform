export interface RtEntityAttributeDto {
  /**
   * Value of a scalar attribute.
   */
  value: OspSimpleScalarType | null;
  /**
   * Values of a compound attribute.
   */
  values: (OspSimpleScalarType | null)[] | null;
}
