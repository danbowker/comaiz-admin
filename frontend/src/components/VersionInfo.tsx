import React, { useEffect, useState } from 'react';
import './VersionInfo.css';

interface VersionData {
  version: string;
  commit: string;
  buildTime: string;
  branch?: string;
}

const VersionInfo: React.FC = () => {
  const [versionData, setVersionData] = useState<VersionData | null>(null);
  const [showDetails, setShowDetails] = useState(false);

  useEffect(() => {
    // Try to get version from build-time environment variable first
    const envVersion = process.env.REACT_APP_VERSION;
    
    // Fetch version.json
    fetch('/version.json')
      .then(response => response.json())
      .then((data: VersionData) => {
        setVersionData(data);
      })
      .catch(() => {
        // Fallback to environment variable if version.json doesn't exist
        if (envVersion) {
          setVersionData({
            version: envVersion,
            commit: 'unknown',
            buildTime: 'unknown'
          });
        }
      });
  }, []);

  if (!versionData) {
    return null;
  }

  return (
    <div className="version-info">
      <span 
        className="version-label"
        onClick={() => setShowDetails(!showDetails)}
        title="Click for details"
      >
        v{versionData.version}
      </span>
      {showDetails && (
        <div className="version-details">
          <div className="version-detail-item">
            <strong>Version:</strong> {versionData.version}
          </div>
          <div className="version-detail-item">
            <strong>Commit:</strong> {versionData.commit.substring(0, 7)}
          </div>
          <div className="version-detail-item">
            <strong>Built:</strong> {new Date(versionData.buildTime).toLocaleString()}
          </div>
          {versionData.branch && (
            <div className="version-detail-item">
              <strong>Branch:</strong> {versionData.branch}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default VersionInfo;
