import React, { createContext, useContext, useState, useEffect, useCallback, useRef } from 'react';
import { useLocation, useSearchParams } from 'react-router-dom';
import { contractsService } from '../services/entityService';
import { Contract } from '../types';

interface ContractSelectionContextType {
  selectedContractId: number | null;
  selectedContract: Contract | null;
  setSelectedContract: (contractId: number | null) => void;
  clearSelectedContract: () => void;
  isLoading: boolean;
  error: string | null;
}

const ContractSelectionContext = createContext<ContractSelectionContextType | undefined>(undefined);

export const ContractSelectionProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [selectedContractId, setSelectedContractId] = useState<number | null>(null);
  const [selectedContract, setSelectedContractState] = useState<Contract | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [searchParams, setSearchParams] = useSearchParams();
  const location = useLocation();
  const loadingRef = useRef<number | null>(null);

  const loadContract = useCallback(async (contractId: number) => {
    // Prevent duplicate loads
    if (loadingRef.current === contractId) {
      return;
    }
    
    loadingRef.current = contractId;
    setIsLoading(true);
    setError(null);
    try {
      const contract = await contractsService.getById(contractId);
      setSelectedContractId(contractId);
      setSelectedContractState(contract);
    } catch (err: any) {
      console.error('Failed to load contract', err);
      setError('Contract not found or access denied');
      setSelectedContractId(null);
      setSelectedContractState(null);
      // Remove invalid contract from URL
      setSearchParams((params) => {
        const newParams = new URLSearchParams(params);
        newParams.delete('contract');
        return newParams;
      }, { replace: true });
    } finally {
      setIsLoading(false);
      loadingRef.current = null;
    }
  }, [setSearchParams]);

  // Initialize from URL on mount and when location changes
  useEffect(() => {
    const contractParam = searchParams.get('contract');
    if (contractParam) {
      const contractId = parseInt(contractParam, 10);
      if (!isNaN(contractId) && contractId !== selectedContractId) {
        loadContract(contractId);
      }
    } else if (selectedContractId !== null) {
      // URL doesn't have contract param but we have a selection - clear it
      setSelectedContractId(null);
      setSelectedContractState(null);
      setError(null);
    }
  }, [location.search, loadContract, searchParams, selectedContractId]);

  const clearSelectedContract = useCallback(() => {
    setSelectedContractId(null);
    setSelectedContractState(null);
    setError(null);
    
    // Remove contract param from URL
    setSearchParams((params) => {
      const newParams = new URLSearchParams(params);
      newParams.delete('contract');
      return newParams;
    }, { replace: true });
  }, [setSearchParams]);

  const setSelectedContract = useCallback((contractId: number | null) => {
    if (contractId === null) {
      clearSelectedContract();
      return;
    }

    // Update URL with contract param using replaceState to avoid history spam
    setSearchParams((params) => {
      const newParams = new URLSearchParams(params);
      newParams.set('contract', contractId.toString());
      return newParams;
    }, { replace: true });
    
    // The useEffect will handle loading the contract when URL changes
  }, [clearSelectedContract, setSearchParams]);

  return (
    <ContractSelectionContext.Provider
      value={{
        selectedContractId,
        selectedContract,
        setSelectedContract,
        clearSelectedContract,
        isLoading,
        error,
      }}
    >
      {children}
    </ContractSelectionContext.Provider>
  );
};

export const useContractSelection = () => {
  const context = useContext(ContractSelectionContext);
  if (context === undefined) {
    throw new Error('useContractSelection must be used within a ContractSelectionProvider');
  }
  return context;
};
