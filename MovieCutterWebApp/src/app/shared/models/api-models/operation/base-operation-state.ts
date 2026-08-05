import { OperationStatus } from '@shared/enums/operation-status';
import { OperationType } from '@shared/enums/operation-type';

export interface BaseOperationState {
  idOperation: number;
  videoName: string;
  operationType: OperationType;
  operationStatus: OperationStatus;
  progressPercentage: number;
}
