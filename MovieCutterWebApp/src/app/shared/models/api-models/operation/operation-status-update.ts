import { OperationStatus } from '@shared/enums/operation-status';

export interface OperationStatusUpdate {
  idOperation: number;
  operationStatus: OperationStatus;
}
