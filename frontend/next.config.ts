import type { NextConfig } from "next";

const useWebpackPolling = process.env.NEXT_DOCKER_DEV_POLLING === "true";

const nextConfig: NextConfig = {

  ...(useWebpackPolling && {
    webpack: (config) => {
      config.watchOptions = {
        poll: 1000,
        aggregateTimeout: 300,
      };
      return config;
    },
  }),
};

export default nextConfig;