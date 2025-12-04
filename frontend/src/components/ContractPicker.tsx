import React, { useState, useEffect, useRef } from 'react';
import { useContractSelection } from '../contexts/ContractSelectionContext';
import { contractsService } from '../services/entityService';
import { Contract, RecordState } from '../types';
import { useShowCompleteContracts } from '../hooks/useFilterPreferences';
import { isContractActive } from '../utils/stateFilter';
import './ContractPicker.css';

const ContractPicker: React.FC = () => {
  const { selectedContract, setSelectedContract, clearSelectedContract, error } = useContractSelection();
  const [isOpen, setIsOpen] = useState(false);
  const [contracts, setContracts] = useState<Contract[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const { showComplete, toggleShowComplete } = useShowCompleteContracts();

  useEffect(() => {
    loadContracts();
  }, []);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
      return () => document.removeEventListener('mousedown', handleClickOutside);
    }
  }, [isOpen]);

  const loadContracts = async () => {
    try {
      setLoading(true);
      const data = await contractsService.getAll();
      setContracts(data);
    } catch (err) {
      console.error('Failed to load contracts', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredContracts = contracts.filter((contract) => {
    const description = contract.description?.toLowerCase() || '';
    const search = searchTerm.toLowerCase();
    const matchesSearch = description.includes(search) || contract.id.toString().includes(search);
    
    // Filter by state unless showComplete is enabled
    if (!showComplete && !isContractActive(contract)) {
      return false;
    }
    
    return matchesSearch;
  });

  const handleSelect = (contract: Contract) => {
    setSelectedContract(contract.id);
    setIsOpen(false);
    setSearchTerm('');
  };

  const handleClear = (e: React.MouseEvent) => {
    e.stopPropagation();
    clearSelectedContract();
    setSearchTerm('');
  };

  const toggleDropdown = () => {
    setIsOpen(!isOpen);
    if (!isOpen) {
      setSearchTerm('');
    }
  };

  return (
    <div className="contract-picker" ref={dropdownRef}>
      <div className="contract-picker-trigger" onClick={toggleDropdown}>
        {selectedContract ? (
          <>
            <span className="selected-contract-name">
              {selectedContract.description || `Contract ${selectedContract.id}`}
            </span>
            <button
              className="clear-btn"
              onClick={handleClear}
              title="Clear selection"
              aria-label="Clear contract selection"
            >
              ×
            </button>
          </>
        ) : (
          <span className="placeholder">All contracts</span>
        )}
        <span className="dropdown-arrow">▼</span>
      </div>

      {error && <div className="contract-picker-error">{error}</div>}

      {isOpen && (
        <div className="contract-picker-dropdown">
          <div className="contract-picker-search">
            <input
              type="text"
              placeholder="Search contracts..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              autoFocus
            />
          </div>
          <div className="contract-picker-filter">
            <label className="contract-picker-filter-toggle">
              <input
                type="checkbox"
                checked={showComplete}
                onChange={toggleShowComplete}
                aria-label="Show complete contracts"
              />
              <span>Show complete</span>
            </label>
          </div>
          <div className="contract-picker-list">
            {loading ? (
              <div className="contract-picker-loading">Loading...</div>
            ) : filteredContracts.length === 0 ? (
              <div className="contract-picker-empty">No contracts found</div>
            ) : (
              filteredContracts.map((contract) => (
                <div
                  key={contract.id}
                  className={`contract-picker-item ${
                    selectedContract?.id === contract.id ? 'selected' : ''
                  } ${contract.state === RecordState.Complete ? 'complete' : ''}`}
                  onClick={() => handleSelect(contract)}
                >
                  <div className="contract-picker-item-name">
                    {contract.description || `Contract ${contract.id}`}
                    {contract.state === RecordState.Complete && (
                      <span className="contract-picker-badge">Complete</span>
                    )}
                  </div>
                  <div className="contract-picker-item-meta">
                    ID: {contract.id}
                    {contract.price && ` • ${contract.price}`}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default ContractPicker;
